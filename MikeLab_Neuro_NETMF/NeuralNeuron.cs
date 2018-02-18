using MikeLab.MicroJSON;
using System;
using System.Collections;
using System.Text;

namespace MikeLab.Neuro
{
    public class NeuralNeuron
    {
        double[] inputs_weight = new double[0];
        double memory_weight = 0.0;
        double hidden = 0.0;
        double threshold = 0.0;
        double alpha = 2.0;

        public double NeuronValue
        {
            get
            {
                return hidden;
            }
        }

        public NeuralNeuron(int inputs, double _memory_weight)
        {
            inputs_weight = new double[inputs];
            for (int i = 0; i < inputs; i++)
            {
                inputs_weight[i] = ((double)0.5);
            }

            memory_weight = _memory_weight;
        }

        public NeuralNeuron(JObject json)
        {
            JArray arr = (JArray)json["Weights"];
            this.inputs_weight = new double[arr.Length()];
            for (int i = 0; i < arr.Length(); i++)
            {
                this.inputs_weight[i] = (double)arr[i];
            }

            this.threshold = (double)json["Threshold"];
            this.alpha = (double)json["Alpha"];
        }

        public void Inputs(double[] inputs)
        {
            double weighted = 0.0;

            for (int i = 0; i < inputs.Length; i++)
            {
                weighted = weighted + ((double)inputs[i] * (double)inputs_weight[i]);
            }

            weighted = weighted + (hidden * memory_weight) + this.threshold;

            hidden = (2 / (1 + Math.Exp(-(this.alpha * weighted)))) - 1.0;


        }
    }
}
