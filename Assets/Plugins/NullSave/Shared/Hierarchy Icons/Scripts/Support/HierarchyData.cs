using System.Collections.Generic;
using UnityEngine;

public class HierarchyData 
{

    public List<Texture> icons;
    public List<Color> colors;
    public List<Rect> rects;
    public Rect selectionRect;

    public HierarchyData()
    {
        icons = new List<Texture>();
        colors = new List<Color>();
        rects = new List<Rect>();
        selectionRect = new Rect(0,0,0,0);
    }

    public HierarchyData(Rect selRect)
    {
        icons = new List<Texture>();
        colors = new List<Color>();
        rects = new List<Rect>();
        selectionRect = selRect;
    }

}
