using System;
using System.Collections.Generic;
using System.Linq;
using Genetics.Gens;

namespace Genetics
{
    class MinConflitsAlgorithm
    {
        private int _maxIter = 5000;
        private readonly int _n;
        private NQueensGen _nQueensGen; //used for the current state
        private readonly Random _rand;

        public MinConflitsAlgorithm(int n = 8)
        {
            _n = n;
            _rand = new Random();
        }

        public void run_algorithm()
        {
            string result = "Failure, no solution was found in "+ _maxIter +" iterations.";
            for (int i = 0; i < _maxIter; i++)
            {
                var conflits = find_conflits(_nQueensGen);
                print_current_state();
                if (_nQueensGen.Fitness!=0)
                {
                    List<int> confIndexs = new List<int>();
                    for (int j = 0; j < conflits.Length; j++)
                    {                    
                        if (conflits[j] != 0)
                        {
                            confIndexs.Add(j);
                        }
                    }
                    var chosenConflitIndex = _rand.Next() % confIndexs.Count;
                    minimze_conflits(chosenConflitIndex,ref _nQueensGen);
                }
                else
                {
                    result = "Solution was found after " + i + " iterations: ";
                    foreach (var cell in _nQueensGen.NQueensPos)
                    {
                        result += cell.ToString()+" ";
                    }
                    break;
                }
            }
            Console.WriteLine(result);
        }
        public void init_current_state()
        {
            List<int> numbers = Enumerable.Range(0, _n).ToList();
            _nQueensGen = new NQueensGen(_n);
            for (int j = 0; j < _n; j++)
            {
                var index = _rand.Next() % numbers.Count;
                _nQueensGen.NQueensPos[j] = numbers[index];
                numbers.RemoveAt(index);
            }
        }

        private int[] find_conflits(NQueensGen currentState)
        {
            int[] conflits = new int[_n];
            currentState.Fitness = 0;
            for (int i = 0; i < _n; i++)
            {
                for (int j = i + 1; j < _n; j++)
                {
                    if (Math.Abs(j - i) == Math.Abs(currentState.NQueensPos[j] - currentState.NQueensPos[i]))
                    {
                        conflits[j] ++;
                        conflits[i] ++;
                        currentState.Fitness += 2;
                    }
                }
            }
            return conflits;
        }
        private void minimze_conflits(int index,ref NQueensGen nQueensGen)
        {
            uint minFit = nQueensGen.Fitness;
            NQueensGen copy = new NQueensGen(_n) {Fitness = nQueensGen.Fitness};
            copy.NQueensPos = new List<int>(nQueensGen.NQueensPos);
            NQueensGen bestGen = new NQueensGen(_n){Fitness = nQueensGen.Fitness,};
            bestGen.NQueensPos = new List<int>(nQueensGen.NQueensPos);
            for (int i = 0; i < _n; i++)
            {
                //swap queens
                if (i== index) continue;
                int temp = copy.NQueensPos[index];
                copy.NQueensPos[index] = copy.NQueensPos[i];
                copy.NQueensPos[i] = temp;

                find_conflits(copy);
                if (copy.Fitness <= minFit)
                {
                    minFit = copy.Fitness;
                    bestGen.Fitness = copy.Fitness;
                    bestGen.NQueensPos = new List<int>(copy.NQueensPos);
                }
                copy.NQueensPos = new List<int>(nQueensGen.NQueensPos);
                copy.Fitness = nQueensGen.Fitness;
            }
            nQueensGen = bestGen;
        }
        private void print_current_state()
        {
            string state = "";
            for (int i = 0; i < _n; i++)
            {
                state += _nQueensGen.NQueensPos[i]+" ";
            }
           Console.WriteLine("Current State: "+ state+" ("+ _nQueensGen.Fitness+")");
        }
    }
}