using MikeLab.MicroJSON;
using System;
using System.Collections;
using System.Text;

namespace MikeLab.Neuro
{
    public class NeuralLayer
    {
        NeuralNeuron[] neurons;
        double[] weighted;

        public int Neurons 
        { 
            get
            {
                return neurons.Length;
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

        public NeuralLayer(NeuralNeuron[] _neurons)
        {
            neurons = _neurons;
            weighted = new double[0];
        }

        public NeuralLayer(JArray arr)
        {
            this.neurons = new NeuralNeuron[arr.Length()];

            for (int i = 0; i < arr.Length(); i++)
            {
                this.neurons[i] = new NeuralNeuron((JObject)arr[i]);
            }

            if (weighted == null)
                weighted = new double[neurons.Length];
        }

        public void Inputs(double[] inputs)
        {

            lock (weighted)
            {
                weighted = new double[neurons.Length];

                for (int i = 0; i < neurons.Length; i++)
                {
                    (neurons[i]).Inputs(inputs);
                    weighted[i] = ((neurons[i]).NeuronValue);
                }
            }
        }

    }
}
