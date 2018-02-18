using MikeLab.MicroJSON;
using System;
using System.Collections;
using System.Text;

namespace MikeLab.Neuro
{
    public class NeuralNetwork
    {
        NeuralLayer[] layers;
        double[] weighted;
        int inputsCount = 0;

        public int Nets
        {
            get
            {
                return layers.Length;
            }
        }

        public double[] NeuronValue
        {
            get
            {
                lock (weighted)
                    return weighted;
            }
        }

        public NeuralNetwork(NeuralLayer[] _nets)
        {
            layers = _nets;
            weighted = new double[0];
        }

        public NeuralNetwork(JObject json)
        {
            this.inputsCount = (int)json["Inputs"];

            this.weighted = new double[this.inputsCount];

            JArray arr = (JArray)json["Layers"];

            this.layers = new NeuralLayer[arr.Length()];

            for (int i = 0; i < arr.Length(); i++)
            {
                this.layers[i] = new NeuralLayer((JArray)arr[i]);
            }
        }

        public void Inputs(double[] inputs)
        {
            if (inputs.Length != this.inputsCount)
                throw new Exception("Error: bad input count, expected " + this.inputsCount.ToString());

            lock (weighted)
            {
            
                double[] passages = new double[inputs.Length];

                for (int i = 0; i < inputs.Length; i++)
                {
                    passages[i] = ((double)inputs[i]);
                }

                weighted = new double[layers[layers.Length - 1].Neurons];

                for (int i = 0; i < layers.Length; i++)
                {
                    (layers[i]).Inputs(passages);
                    passages = (layers[i]).NeuronValue;
                }

                weighted = passages;
            }

        }
    }
}
