using NeatNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functionality
{
    public static class NetworkHolder
    {
        public static LoadedNetworkType loadedNetwork;

        public enum LoadedNetworkType
        {
            autoencoder = 1,
            Gans = 2,
            ReverseDiffusor = 3,
        }


        public static NN autoencoder, generative, discriminative, reverseDiffusor;
        public static int reverseDiffusorDiffusions;
    }
}
