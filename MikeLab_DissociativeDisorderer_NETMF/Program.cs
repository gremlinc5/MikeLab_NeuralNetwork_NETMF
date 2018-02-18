using System;
using Microsoft.SPOT;

using MikeLab.MicroJSON;
using MikeLab.Neuro;
using MikeLab.PhysicalMassMemory;
using Microsoft.SPOT.IO;
using System.Threading;

namespace MikeLab_DissociativeDisorderer_NETMF
{
    public class Program
    {

        public static void Main()
        {
            MikeLab.PhysicalMassMemory.PhysicalDrive.Init();

            while (PhysicalDrive.ReadyRead != true)
            {
                Thread.Sleep(222);
            }

            string rootDirectory = VolumeInfo.GetVolumes()[0].RootDirectory;
            string configuration = rootDirectory + @"\neuralnet.conf";

            Debug.Print("Read Bytes from SD Card: " + PhysicalDrive.ReadFileOnSDCardAsString(configuration, out configuration).ToString());

            var jdom = (JObject)JsonHelpers.Parse(configuration);

            NeuralNetwork neuralnet = new NeuralNetwork(jdom);

            neuralnet.Inputs(new double[] { 0.0, 0.0 });
            Debug.Print("Value for 0 0 is " + neuralnet.NeuronValue[0].ToString());

            neuralnet.Inputs(new double[] { 1.0, 0.0 });
            Debug.Print("Value for 1 0 is " + neuralnet.NeuronValue[0].ToString());

            neuralnet.Inputs(new double[] { 0.0, 1.0 });
            Debug.Print("Value for 0 1 is " + neuralnet.NeuronValue[0].ToString());

            neuralnet.Inputs(new double[] { 1.0, 1.0 });
            Debug.Print("Value for 1 1 is " + neuralnet.NeuronValue[0].ToString());

        }
    }
}
