using NeatNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Functionality
{
    public static class NetworkHolder
    {
        public static NetworkType loadedNetwork;

        public enum NetworkType
        {
            autoencoder = 1,
            Gans = 2,
            ReverseDiffusor = 3,
            DiffusionSubtractor = 4,
        }


        public static NN autoencoder, generative, discriminative, reverseDiffusor, diffusionSubtractor;
        public static int reverseDiffusorDiffusions;

        public static NetworkType AskForNetworkType()
        {
            return (NetworkType)Enum.Parse(typeof(NetworkType), AskForNetworkTypeName());
        }

        public static string AskForNetworkTypeName()
        {
            string[] networkTypesName = Enum.GetNames(typeof(NetworkType));
            while (true)
                foreach (string networkType in networkTypesName)
                {
                    if (MessageBox.Show(networkType + "?", "Choose a network type", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        return networkType;
                }
        }
    }
}
