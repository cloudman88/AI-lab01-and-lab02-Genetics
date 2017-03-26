using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genetics.Gens;

namespace Genetics
{
    class BinPackingGenetics : GeneticsAlgorithms<VolumesGen>
    {
        private readonly MutationOperator _mutationOperator;
        private readonly List<int> _volumes;
        private readonly int _containerCapacity;
        private readonly int _lowerBound;

        public BinPackingGenetics(List<int> volumes, int containerCapacity, MutationOperator mutationOperator, CrossoverMethod method,bool isTour) : base(method,isTour)
        {
            _mutationOperator = mutationOperator;
            _volumes = new List<int>(volumes);
            _containerCapacity = containerCapacity;
            _lowerBound = (int) Math.Ceiling((decimal) (volumes.Sum()/containerCapacity));
        }

        public override void init_population()
        {
            population.Clear();
            buffer.Clear();
            for (int i = 0; i < GaPopSize; i++)
            {
                List<int> emptyVols = Enumerable.Repeat(-1, _volumes.Count).ToList();
                VolumesGen volumesGen = new VolumesGen(emptyVols);
                List<int> vols = new List<int>(_volumes);
                for (int j = 0; j < _volumes.Count; j++)
                {
                    var index = Rand.Next() % vols.Count;
                    volumesGen.Volumes[j] = vols[index];
                    vols.RemoveAt(index);
                }

                population.Add(volumesGen);
                buffer.Add(new VolumesGen(emptyVols));
            }

        }

        protected override void calc_fitness()
        {
            BinPackingAlgorithm bpa = new BinPackingAlgorithm(_volumes,_containerCapacity);
            for (int i = 0; i < GaPopSize; i++)
            {
                bpa.Volumes = new List<int>(population[i].Volumes);
                bpa.Bins.Clear();
                bpa.run_first_fit_algo(false);
                population[i].Fitness = (uint) (bpa.Bins.Count - _lowerBound);
            }
        }
        protected override void Mutate(VolumesGen member)
        {
            List<int> nQlist = member.Volumes.ToList();
            int volsSize = _volumes.Count;
            int ipos1 = Rand.Next() % volsSize;
            int ipos2;
            switch (_mutationOperator)
            {
                case MutationOperator.Displacement:
                    ipos2 = Rand.Next() % (volsSize - ipos1) + ipos1;
                    if (ipos2 - ipos1 != 7)
                        member.Volumes = MutOpDisplacement(ipos1, ipos2, nQlist);
                    break;
                case MutationOperator.Exchange:
                    ipos2 = Rand.Next() % volsSize;
                    member.Volumes = MutOpExchange(ipos1, ipos2, nQlist);
                    break;
                case MutationOperator.Insertion:
                    ipos2 = Rand.Next() % volsSize;
                    member.Volumes = MutOpInsertion(ipos1, ipos2, nQlist);
                    break;
                case MutationOperator.SimpleInversion:
                    ipos2 = Rand.Next() % (volsSize - ipos1) + ipos1;
                    member.Volumes = MutOpSimpleInversion(ipos1, ipos2, nQlist);
                    break;
                case MutationOperator.Inversion:
                    ipos2 = Rand.Next() % (volsSize - ipos1) + ipos1;
                    List<int> temp = MutOpSimpleInversion(ipos1, ipos2, nQlist);
                    member.Volumes = MutOpDisplacement(ipos1, ipos2, temp.ToList());
                    break;
                case MutationOperator.Scramble:
                    ipos2 = Rand.Next() % (volsSize - ipos1) + ipos1;
                    member.Volumes = MutOpScramble(ipos1, ipos2, nQlist);
                    break;
                default:
                    member.Volumes[ipos1] = Rand.Next() % volsSize;
                    break;
            }
        }
        private List<int> MutOpDisplacement(int ipos1, int ipos2, List<int> nQlist)
        {
            int volsSize = _volumes.Count;
            int gapPos = Rand.Next() % (volsSize - (ipos2 - ipos1));
            List<int> chosenPart = new List<int>();
            List<int> theRest = new List<int>(nQlist);
            for (int i = ipos1, count = 0; i < ipos2 + 1; i++, count++)
            {
                chosenPart.Add(nQlist[i]);
                theRest.RemoveAt(i - count);
            }
            for (int i = gapPos, j = 0; i < chosenPart.Count + gapPos; i++, j++)
            {
                theRest.Insert(i, chosenPart[j]);
            }
            return theRest;
        }
        private List<int> MutOpExchange(int ipos1, int ipos2, List<int> nQlist)
        {
            int temp = nQlist[ipos1];
            nQlist[ipos1] = nQlist[ipos2];
            nQlist[ipos2] = temp;
            return nQlist;
        }
        private List<int> MutOpInsertion(int ipos1, int ipos2, List<int> nQlist)
        {
            int selectedVal = nQlist[ipos1];
            nQlist[ipos1] = -1;
            int fix = (ipos2 == 0) ? 0 : 1;
            nQlist.Insert(ipos2 + fix, selectedVal);
            nQlist.Remove(-1);
            return nQlist;
        }
        private List<int> MutOpSimpleInversion(int ipos1, int ipos2, List<int> nQlist)
        {
            List<int> chosenPart = new List<int>();
            for (int i = ipos1; i < ipos2 + 1; i++)
            {
                chosenPart.Add(nQlist[i]);
            }
            chosenPart.Reverse();
            for (int i = ipos1, j = 0; i < ipos2 + 1; i++, j++)
            {
                nQlist[i] = chosenPart[j];
            }
            return nQlist;
        }
        private List<int> MutOpScramble(int ipos1, int ipos2, List<int> nQlist)
        {
            List<int> chosenPart = new List<int>();
            for (int i = ipos1; i < ipos2 + 1; i++)
            {
                chosenPart.Add(nQlist[i]);
            }

            //scramble the chosen part
            var chosenPartSize = chosenPart.Count;
            for (int i = 0; i < chosenPartSize / 2; i++)
            {
                int rand1 = Rand.Next() % chosenPartSize;
                int rand2 = Rand.Next() % chosenPartSize;
                var temp = chosenPart[rand1];
                chosenPart[rand1] = chosenPart[rand2];
                chosenPart[rand2] = temp;
            }

            for (int i = ipos1, j = 0; i < ipos2 + 1; i++, j++)
            {
                nQlist[i] = chosenPart[j];
            }
            return nQlist;
        }
        protected override void mate_by_method(VolumesGen bufGen, VolumesGen gen1, VolumesGen gen2)
        {
            List<int> nQlist1 = gen1.Volumes.ToList();
            List<int> nQlist2 = gen2.Volumes.ToList();
            int volsSize = _volumes.Count;
            int ipos1 = Rand.Next() % volsSize;
            //int qpos2 = Rand.Next() % (volsSize - qpos1) + qpos1;
            switch (crossoverMethod)
            {
                case CrossoverMethod.PMX:
                    int val1 = nQlist1[ipos1];
                    int val2 = nQlist2[ipos1];
                    var index1 = nQlist1.IndexOf(val2);
                    var index2 = nQlist2.IndexOf(val1);
                    nQlist1[index1] = val1;
                    nQlist2[index2] = val2;
                    nQlist1[ipos1] = val2;
                    nQlist2[ipos1] = val1;
                    bufGen.Volumes = nQlist2;
                    break;
                case CrossoverMethod.OX:
                    List<int> temp = Enumerable.Repeat(-1, volsSize).ToList();
                    List<int> numbers = Enumerable.Range(0, volsSize).ToList();

                    for (int i = 0; i < volsSize / 2; i++)
                    {
                        temp[numbers[ipos1]] = nQlist1[numbers[ipos1]];
                        numbers.RemoveAt(ipos1);
                        ipos1 = Rand.Next() % numbers.Count;
                    }
                    int j = 0;
                    foreach (var val in nQlist2)
                    {
                        if (!temp.Contains(val))
                        {
                            while (j < temp.Count && temp[j] != -1)
                            {
                                j++;
                            }
                            if (j != temp.Count) temp[j] = val;
                        }
                    }
                    bufGen.Volumes = temp;
                    break;
                case CrossoverMethod.CX:
                    List<int> cycle = new List<int>();
                    List<int> result = Enumerable.Repeat(-1, volsSize).ToList();
                    int k = 1;
                    var start = nQlist1[0];
                    cycle.Add(start);
                    var end = nQlist2[0];
                    if (start != end)
                    {
                        cycle.Add(nQlist2[0]);
                        while (k < volsSize)
                        {
                            var endIndex = nQlist1.IndexOf(end);
                            end = nQlist2[endIndex];
                            if (start == end) break;
                            cycle.Add(end);
                            k++;
                        }
                    }

                    foreach (var val in cycle)
                    {
                        var index = nQlist1.IndexOf(val);
                        result[index] = val;
                    }
                    k = 0;
                    while (k < volsSize)
                    {
                        if (result[k] == -1)
                        {
                            result[k] = nQlist2[k];
                        }
                        k++;
                    }
                    bufGen.Volumes = result;
                    break;
                case CrossoverMethod.ER:
                    Dictionary<int, List<int>> neighborsDic1 = new Dictionary<int, List<int>>();
                    Dictionary<int, List<int>> neighborsDic2 = new Dictionary<int, List<int>>();
                    Dictionary<int, List<int>> neighborsDicMerged = new Dictionary<int, List<int>>();

                    for (int i = 0; i < volsSize; i++)
                    {
                        List<int> neighbors1 = new List<int>();
                        List<int> neighbors2 = new List<int>();
                        int right = (i + 1) % volsSize;
                        int left = i - 1;
                        if (left < 0) left = volsSize - 1;
                        neighbors1.Add(nQlist1[right]);
                        neighbors1.Add(nQlist1[left]);
                        neighbors2.Add(nQlist2[right]);
                        neighbors2.Add(nQlist2[left]);
                        neighborsDic1.Add(nQlist1[i], neighbors1);
                        neighborsDic2.Add(nQlist2[i], neighbors2);
                    }

                    // join lists for all vals
                    int p = 0;
                    while (p < volsSize)
                    {
                        var list1 = neighborsDic1[p];
                        var list2 = neighborsDic2[p];
                        neighborsDicMerged.Add(p, list1.Union(list2).ToList());
                        p++;
                    }

                    List<int> res = new List<int>();
                    int link = nQlist1[0];
                    res.Add(link);

                    numbers = Enumerable.Range(0, volsSize).ToList();
                    numbers.Remove(link);
                    RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                    while (res.Count < volsSize)
                    {
                        var neighbors = neighborsDicMerged[link];

                        int minNeighbors = volsSize;
                        int tempIndex = 0;
                        if (neighbors.Count > 0)
                        {
                            for (int i = 0; i < neighbors.Count; i++)
                            {
                                if (neighborsDicMerged[neighbors[i]].Count < minNeighbors)
                                {
                                    minNeighbors = neighborsDicMerged[neighbors[i]].Count;
                                    tempIndex = i;
                                }
                            }
                            link = neighbors[tempIndex];
                            res.Add(link);
                            RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                            numbers.Remove(link);
                        }
                        else
                        {
                            var index = Rand.Next() % numbers.Count;
                            link = numbers[index];
                            numbers.Remove(link);
                            res.Add(link);
                            RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                        }
                    }
                    bufGen.Volumes = res;

                    break;
            }
        }
        private void RemoveNeigbhorFromAllLists(Dictionary<int, List<int>> neighborsDicMerged, int target)
        {
            for (int i = 0; i < _volumes.Count; i++)
            {
                var x = neighborsDicMerged[i];
                if (x.Contains(target))
                {
                    x.Remove(target);
                }
            }
        }
        protected override Tuple<string, uint> get_best_gen_details(VolumesGen gen)
        {
            string str = "";
            foreach (var cell in gen.Volumes)
            {
                str += cell.ToString() + " ";
            }
            return new Tuple<string, uint>(str, gen.Fitness);
        }
    }
}
