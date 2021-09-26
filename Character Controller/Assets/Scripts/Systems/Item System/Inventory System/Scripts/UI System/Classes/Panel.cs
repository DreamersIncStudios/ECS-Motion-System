using System.Collections;
using System.Collections.Generic;
using Dreamers.Global;
using System;
using UnityEngine;
using UnityEngine.UI;
using Dreamers.InventorySystem.Base;

namespace Dreamers.InventorySystem.UISystem {
    public abstract class Panel 
    {
        public GameObject Top;

        public  UIManager Manager { get { return UIManager.instance; } }
        public Vector2 Size { get; private set; }
        public Vector2 Position { get; private set; }

        public void Setup(Vector2 Size, Vector2 Position) {
            this.Size = Size;
            this.Position = Position;
        }
        public abstract GameObject CreatePanel(Transform Parent);
        public abstract void DestoryPanel();

        public abstract void Refresh();

    }
}