using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;

namespace RTS_test
{

    public class TileData
    {
        string name;
        Texture2D texture;
        bool isSolid;

        public TileData(string name, Texture2D texture, bool isSolid)
        {
            this.name = name;
            this.texture = texture;
            this.isSolid = isSolid;
        }

        public string Name { get { return name; } }
        public Texture2D Texture { get { return texture; } }
        public bool IsSolid { get { return isSolid; } }
    }

    public class TileManager
    {
        private List<TileData> registeredTiles = new List<TileData>();
        private Dictionary<string, UInt16> tileIDs = new Dictionary<string, UInt16>();

        public TileManager()
        {
        }

        public UInt16 registerTile(TileData tileData)
        {
            UInt16 id = (UInt16)registeredTiles.Count();

            registeredTiles.Add(tileData);
            tileIDs.Add(tileData.Name, id);

            return id;
        }

        public UInt16 getIDByName(string name)
        {
            if (!tileIDs.ContainsKey(name))
                throw new Exception("No tile called " + name + " found!");

            return tileIDs[name];
        }

        public TileData getTile(UInt16 id)
        {
            if (id >= registeredTiles.Count())
                throw new Exception("Unregistered tileID(" + id.ToString() + ")!");

            return registeredTiles[id];
        }
    }
}
