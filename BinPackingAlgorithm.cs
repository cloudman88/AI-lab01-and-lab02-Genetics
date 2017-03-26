using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Genetics
{
    class BinPackingAlgorithm
    {
        private readonly int _containerSize;
        private readonly int _lowerBound;
        public ObservableCollection<List<int>> Bins;
        public List<int> Volumes;

        public BinPackingAlgorithm( List<int> volumes, int containerSize )
        {
            _containerSize = containerSize;
            Volumes = new List<int>(volumes);
            Bins = new ObservableCollection<List<int>>();
            _lowerBound = (int) Math.Ceiling((decimal) (Volumes.Sum()/_containerSize));
        }

        private void print_result()
        {
            string volumesString = "";
            foreach (var vol in Volumes)
            {
                volumesString += vol + " ";
            }
            Console.WriteLine("Input volumes: " + volumesString);
            Console.WriteLine("Lower bound is  "+ _lowerBound + " bins for container sized "+_containerSize);
            Console.WriteLine("FirstFit result "+Bins.Count+" bins");
            int i = 0;
            foreach (var bin in Bins)
            {
                volumesString = "";
                foreach (var vol in bin)
                {
                    volumesString += vol + " ";
                }
                i++;
                Console.WriteLine("bin#"+i+"> " +volumesString);
            }
        }
        public void run_first_fit_algo(bool printResult = true)
        {
            List<int> copyVolumes = new List<int>(Volumes);
            Bins.Add(new List<int>());
            while (copyVolumes.Count > 0)
            {
                var vol = copyVolumes.First();
                copyVolumes.RemoveAt(0);
                bool wasInserted = insert_to_first_availble_bin(vol);
                if (wasInserted == false)
                {
                    List<int> bin = new List<int>();
                    bin.Add(vol);
                    Bins.Add(bin);
                }
            }
            if (printResult == true) print_result();
        }
        private bool insert_to_first_availble_bin(int vol)
        {
            bool wasInserted = false;
            foreach (var bin in Bins)
            {
                if (bin.Sum() + vol <= _containerSize)
                {
                    bin.Add(vol);
                    wasInserted = true;
                    break;
                }
            }
            return wasInserted;
        }
    }
}