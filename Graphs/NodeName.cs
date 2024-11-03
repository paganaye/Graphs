using System;
using System.Collections.Generic;

namespace Graphs
{
    public static class NodeName
    {
        public const int MaxNodeCount = 5000; 
        private static readonly string[] NodeNames = new string[MaxNodeCount];
        private static readonly Dictionary<string, int> NodeIndices = new Dictionary<string, int>(); // A-Z, AA-ZZ, etc.

        static NodeName()
        {
            // Pré-calcul des noms de nœuds et des indices correspondants
            for (int i = 0; i < MaxNodeCount; i++)
            {
                string nodeName = GetNodeName(i);
                NodeNames[i] = nodeName;
                NodeIndices[nodeName] = i; // Remplissage du dictionnaire pour recherche rapide
            }
        }

        private static string GetNodeName(int index)
        {
            index++; // Passage en 1-based pour correspondre à la logique d'Excel
            string name = string.Empty;
            while (index > 0)
            {
                index--; // Passage à 0-based pour obtenir la bonne lettre
                name = (char)('A' + (index % 26)) + name;
                index /= 26;
            }
            return name;
        }

        public static string Get(int nodeIndex)
        {
            if (nodeIndex < 0 || nodeIndex >= MaxNodeCount)
                throw new ArgumentOutOfRangeException(nameof(nodeIndex), "Index out of range for node naming.");
            return NodeNames[nodeIndex];
        }

        public static int GetIndex(string nodeName)
        {
            if (NodeIndices.TryGetValue(nodeName, out int index))
            {
                return index;
            }
            throw new ArgumentException("Node name is out of range or invalid.", nameof(nodeName));
        }
    }
}