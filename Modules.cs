using BrokeProtocolClient.utils;
using BrokeProtocolClient.modules;
using BrokeProtocolClient.modules.combat;
using BrokeProtocolClient.modules.player;
using BrokeProtocolClient.modules.movement;
using BrokeProtocolClient.modules.render;
using BrokeProtocolClient.modules.misc;
using BrokeProtocolClient.modules.exploit;
using System.Collections.Generic;
using System;
using BrokeProtocolClient.modules.exploit.botter;

namespace BrokeProtocolClient.modules
{
    class Modules
    {
        public static Modules instance;

        private readonly List<Category> categories = new List<Category>();

        private readonly List<Module> modules = new List<Module>();
        private readonly List<Module> active = new List<Module>();
        private readonly Dictionary<Category, List<Module>> groups = new Dictionary<Category, List<Module>>();

        public Modules()
        {
            instance = this;
            init();
        }

        public void init()
        {
            initCombat();
            initPlayer();
            initMovement();
            initRender();
            initExploit();
            initMisc();
        }

        public void registerCategory(Category category)
        {
            categories.Add(category);
        }

        public void addActive(Module module)
        {
            if (!active.Contains(module))
            {
                active.Add(module);
            }
        }

        public List<Category> getCategories()
        {
            return categories;
        }

        public Module getModule(Type type)
        {
            foreach (Module module in modules)
            {
                if (module.GetType() == type) return module;
            }
            return null;
        }

        public Module getActiveModule(Type type)
        {
            foreach (Module module in active)
            {
                if (module.GetType() == type) return module;
            }
            return null;
        }

        public List<Module> getModules()
        {
            return modules;
        }

        public List<Module> getModulesInCategory(Category category)
        {
            List<Module> list = new List<Module>();
            foreach (Module module in modules)
            {
                if (module.category == category)
                {
                    list.Add(module);
                }
            }
            return list;
        }

        public List<Module> getActive()
        {
            return active;
        }

        public void removeActive(Module module)
        {
            active.Remove(module);
        }

        public void disableAll()
        {
            foreach (Module module in modules)
            {
                if (module.enabled) module.toggle();
            }
        }

        public void add(Module module)
        {
            modules.Add(module);
        }


        private void initCombat()
        {
            add(new Aimbot());
            add(new Recoil());
            add(new NoSpread());
            add(new MeleeReach());
            //add(new Spinbot());
            //add(new Antiaim());
            add(new Kamikaze());
        }


        private void initPlayer()
        {
            add(new FovChanger());
            add(new NoRestrain());
            add(new NoWeightLimit());
            add(new AutoCrack());
            add(new AutoSteal());
            add(new AutoHeal());
            add(new AutoCollect());
            add(new AutoDefuse());
            add(new AutoDrop());
        }

        private void initMovement()
        {
            add(new Speed());
            //add(new NoFall());
            add(new Noclip());
        }

        private void initRender()
        {
            add(new HUD());
            add(new Esp());
            add(new Chams());
            add(new Nametags());
            add(new Skeleton());
            add(new Freecam());
            add(new Trajectories());
            add(new Fog());
            add(new DamageNumbers());
        }

        private void initExploit()
        {
            add(new BruteForcer());
            //add(new InteriorLoader());
            add(new ChatSpoofer());
            add(new Botter());
            add(new BotSpammer());
            add(new Teleport());
            add(new AutoStand());
            add(new AutoBotTrade());
            add(new AutoHackAppartment());
            add(new VehicleSpeed());
	    add(new HitboxExpander());
            //add(new Give());
            add(new ServerCrasher());
        }

        private void initMisc()
        {
            add(new BetterChat());
            add(new Notifier());
            add(new ChatSpammer());
            add(new TradeSpammer());
            add(new AlertSpammer());
            add(new HandsUpSpammer());
            add(new MapSaver());
            add(new Spoofer());
            add(new PacketManager());
        }
    }
}
