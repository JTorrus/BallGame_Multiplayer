using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace PosLibrary
{
    [Serializable]
    public class Position 
    {
        public double posX { get; set; }
        public double posY { get; set; }

        public Position(double posX, double posY)
        {
            this.posX = posX;
            this.posY = posY;
        }

        public static byte[] Serialize(object obj)
        {
            byte[] bytesPos;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                bytesPos = ms.ToArray();
            }

            return bytesPos;
        }

        public static object Deserialize(byte[] param)
        {
            object obj = null;
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                obj = (br.Deserialize(ms));
            }

            return obj;
        }
    }
}
