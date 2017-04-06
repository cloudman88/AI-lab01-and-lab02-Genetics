using System;
using System.Collections.Generic;
using System.Linq;
using Genetics.GeneticsAlgorithms;

namespace Genetics.NQueens
{
    enum MutationOperator
    {
        Displacement,
        Exchange,
        Insertion,
        SimpleInversion,
        Inversion,
        Scramble
    }
    class NQueens : GeneticsAlgorithms<NQueensGen>
    {
        private readonly int _n;
        private readonly MutationOperator _mutationOperator;

        public NQueens(int n, MutationOperator mutationOperator,CrossoverMethod method,SelectionMethod selectionMethod) : base(method , selectionMethod)
        {
            _n = n;
            _mutationOperator = mutationOperator;
        }

        public override void init_population()
        {
            Population.Clear();
            Buffer.Clear();
            for (int i = 0; i < GaPopSize; i++)
            {
                NQueensGen queensGen = new NQueensGen(_n);
                List<int> numbers = Enumerable.Range(0, _n).ToList();

                for (int j = 0; j < _n; j++)
                {
                    var index = Rand.Next() % numbers.Count;
                    queensGen.NQueensPos[j] = numbers[index];
                    numbers.RemoveAt(index);
                }

                Population.Add(queensGen);
                Buffer.Add(new NQueensGen(_n));
            }
        }

        protected override void calc_fitness()
        {
            for (int i = 0; i < GaPopSize; i++)
            {
                uint fitness = 0;
                fitness += calc_crosses_hit(Population[i].NQueensPos);                
                Population[i].Fitness = fitness;
            }
        }
        private uint calc_crosses_hit(List<int> nQueensGen)
        {
            uint crossFit = 0;
            for (int i = 0; i < _n; i++)
            {
                for (int j = i+1; j < _n; j++)
                {
                    if (Math.Abs(j - i) == Math.Abs(nQueensGen[j] - nQueensGen[i]))
                    {
                        crossFit += 20;
                    }
                }
            }
            return crossFit;
        }
        protected override void Mutate(NQueensGen member)
        {            
            List<int> nQlist = member.NQueensPos.ToList();
            int ipos1 = Rand.Next() % _n;
            int ipos2;
            switch (_mutationOperator)
            {
                    case MutationOperator.Displacement:
                        ipos2 = Rand.Next() % (_n-ipos1) + ipos1;
                        if (ipos2-ipos1!=7)
                            member.NQueensPos = MutOpDisplacement(ipos1,ipos2,nQlist);
                    break;
                    case MutationOperator.Exchange:
                        ipos2 = Rand.Next() % _n;
                        member.NQueensPos = MutOpExchange(ipos1, ipos2, nQlist);
                    break;
                    case MutationOperator.Insertion:
                        ipos2 = Rand.Next() % _n; 
                        member.NQueensPos = MutOpInsertion(ipos1,ipos2,nQlist);
                    break;
                    case MutationOperator.SimpleInversion:
                        ipos2 = Rand.Next() % (_n - ipos1) + ipos1;
                        member.NQueensPos = MutOpSimpleInversion(ipos1, ipos2, nQlist);
                    break;
                    case MutationOperator.Inversion:
                        ipos2 = Rand.Next() % (_n - ipos1) + ipos1;
                        List<int> temp = MutOpSimpleInversion(ipos1, ipos2, nQlist);
                        member.NQueensPos = MutOpDisplacement(ipos1, ipos2, temp.ToList());
                    break;
                    case MutationOperator.Scramble:
                        ipos2 = Rand.Next() % (_n - ipos1) + ipos1;
                        member.NQueensPos = MutOpScramble(ipos1, ipos2, nQlist);
                    break;
                    default:
                        member.NQueensPos[ipos1] = Rand.Next()%_n;
                    break;
            }
        }
        private List<int> MutOpDisplacement(int ipos1,int ipos2, List<int> nQlist)
        {
            int gapPos = Rand.Next() % (_n - (ipos2 - ipos1));
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
                int rand1 = Rand.Next()% chosenPartSize;
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

        protected override void mate_by_method(NQueensGen bufGen, NQueensGen gen1, NQueensGen gen2)
        {
            List<int> nQlist1 = gen1.NQueensPos.ToList();
            List<int> nQlist2 = gen2.NQueensPos.ToList();
            int ipos1 = Rand.Next() % _n;
            switch (CrosMethod)
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
                    bufGen.NQueensPos = nQlist2;
                    break;
                case CrossoverMethod.OX:
                    List<int> temp = Enumerable.Repeat(-1, _n).ToList();
                    List<int> numbers = Enumerable.Range(0, _n).ToList();

                    for (int i = 0; i < _n/2; i++)
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
                            if (j!= temp.Count) temp[j] = val;
                        }
                    }
                    bufGen.NQueensPos = temp;
                    break;
                case CrossoverMethod.CX:
                    List<int> cycle = new List<int>();
                    List<int> result = Enumerable.Repeat(-1, _n).ToList();
                    int k = 1;
                    var start = nQlist1[0];
                    cycle.Add(start);
                    var end = nQlist2[0];
                    if (start != end)
                    {
                        cycle.Add(nQlist2[0]);
                        while (k < _n)
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
                    while (k < _n)
                    {
                        if (result[k] == -1)
                        {
                            result[k] = nQlist2[k];
                        }
                        k++;
                    }
                    bufGen.NQueensPos = result;
                    break;
                case CrossoverMethod.ER:
                    Dictionary<int,List<int>> neighborsDic1 = new Dictionary<int, List<int>>();
                    Dictionary<int, List<int>> neighborsDic2 = new Dictionary<int, List<int>>();
                    Dictionary<int, List<int>> neighborsDicMerged = new Dictionary<int, List<int>>();

                    for (int i = 0; i < _n; i++)
                    {
                        List<int> neighbors1 = new List<int>();
                        List<int> neighbors2 = new List<int>();
                        int right = (i + 1) % _n;
                        int left = i - 1;
                        if (left < 0) left = _n - 1;
                        neighbors1.Add(nQlist1[right]);
                        neighbors1.Add(nQlist1[left]);
                        neighbors2.Add(nQlist2[right]);
                        neighbors2.Add(nQlist2[left]);
                        neighborsDic1.Add(nQlist1[i], neighbors1);
                        neighborsDic2.Add(nQlist2[i], neighbors2);
                    }

                    // join lists for all vals
                    int p = 0;
                    while (p < _n)
                    {
                        var list1 = neighborsDic1[p];
                        var list2 = neighborsDic2[p];
                        neighborsDicMerged.Add(p, list1.Union(list2).ToList());
                        p++;
                    }

                    List<int> res = new List<int>();
                    int link = nQlist1[0];
                    res.Add(link);

                    numbers = Enumerable.Range(0, _n).ToList();
                    numbers.Remove(link);
                    RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                    while (res.Count < _n)
                    {                        
                        var neighbors = neighborsDicMerged[link];

                        int minNeighbors = _n;
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
                            var index = Rand.Next()%numbers.Count;
                            link = numbers[index];
                            numbers.Remove(link);
                            res.Add(link);
                            RemoveNeigbhorFromAllLists(neighborsDicMerged, link);
                        }
                    }
                    bufGen.NQueensPos = res;

                    break;
            }
        }
        private void RemoveNeigbhorFromAllLists(Dictionary<int, List<int>> neighborsDicMerged,int target)
        {
            for (int i = 0; i < _n; i++)
            {
                var x = neighborsDicMerged[i];
                if (x.Contains(target))
                {
                    x.Remove(target);
                }
            }
        }
        protected override Tuple<string, uint> get_best_gen_details(NQueensGen gen)
        {
            string str = "";
            foreach (var cell in gen.NQueensPos)
            {
                str += cell.ToString()+" ";
            }
            return new Tuple<string, uint>(str, gen.Fitness);
        }

        protected override NQueensGen get_new_gen()
        {
            return new NQueensGen(_n);
        }

        protected override int calc_distance(NQueensGen gen1, NQueensGen gen2)
        {
            throw new NotImplementedException("none");
        }
    }
}