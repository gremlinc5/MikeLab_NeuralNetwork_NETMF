using AForge.Neuro;
using AForge.Neuro.Learning;
using MikeLab.MicroJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikeLab_DissociativeDisorderer
{
    class Program
    {
        static void Main(string[] args)
        {
            double learningRate = 0.1;
            double momentum = 0.0;
            double sigmoidAlphaValue = 1.0;
            
            // two hidden layers and one output layer
            int[] neuronsInFirstLayer = new int[] {2, 2, 1};

            // prepare learning data
            double[][] input = new double[4][];
            double[][] output = new double[4][];

            input[0] = new double[2];
            input[0][0] = 0.0;
            input[0][1] = 0.0;
            input[1] = new double[2];
            input[1][0] = 1.0;
            input[1][1] = 0.0;
            input[2] = new double[2];
            input[2][0] = 0.0;
            input[2][1] = 1.0;
            input[3] = new double[2];
            input[3][0] = 1.0;
            input[3][1] = 1.0;

            output[0] = new double[1];
            output[0][0] = 0.0;
            output[1] = new double[1];
            output[1][0] = 0.0;
            output[2] = new double[1];
            output[2][0] = 0.0;
            output[3] = new double[1];
            output[3][0] = 1.0;

            // create multi-layer neural network
            ActivationNetwork    network = new ActivationNetwork(new BipolarSigmoidFunction( sigmoidAlphaValue ), 2, neuronsInFirstLayer);
            

            // create teacher
            BackPropagationLearning teacher = new BackPropagationLearning( network );
            
            // set learning rate and momentum
            teacher.LearningRate = learningRate;
            teacher.Momentum     = momentum;

            network.Randomize();

            // loop
            int i = 0;
            while (i < 100000)
            {
                // run epoch of learning procedure
                double error = teacher.RunEpoch( input, output ) / 4;
                i++;
            }

            Console.WriteLine("Value Input (0, 0): " + network.Compute(new double[2]{0.0, 0.0})[0].ToString());
            Console.WriteLine("Value Input (0, 1): " + network.Compute(new double[2]{0.0, 1.0})[0].ToString());
            Console.WriteLine("Value Input (1, 0): " + network.Compute(new double[2]{1.0, 0.0})[0].ToString());
            Console.WriteLine("Value Input (1, 1): " + network.Compute(new double[2]{1.0, 1.0})[0].ToString());

            JObject json = new JObject();

            json = network.GetJson();

            Console.ReadLine();

            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\neuralnet.conf", JsonHelpers.Serialize(json));
        }
    }
}
