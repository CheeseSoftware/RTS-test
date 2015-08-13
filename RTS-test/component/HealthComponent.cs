using Artemis.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_test.component
{
    public class HealthComponent : IComponent
    {
        float health;
        float maxHealth;
        bool visible = false;

        public HealthComponent(float health, float maxHealth = 100)
        {
            this.health = health;
            this.maxHealth = maxHealth;
        }

        public float Health { get { return health; } set { health = value; } }

        public float MaxHealth { get { return maxHealth; } set { maxHealth = value; } }

        public bool Visible { get { return visible; } set { visible = value; } }
    }
}
