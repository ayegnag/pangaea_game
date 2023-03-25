using System;

namespace KOI
{
	public class EntityId : EventArgs
	{
        public string Type;
        public int Id;

        public EntityId(string type, int id) {
            Type = type;
            Id = id;
        }
	}
}
