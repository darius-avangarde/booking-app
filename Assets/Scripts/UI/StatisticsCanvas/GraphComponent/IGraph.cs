using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraph
{
    List<float> Data
    {
        get;
        set;
    }

    string XAxisLabel
    {
        get;
        set;
    }

    string YAxisLabel
    {
        get;
        set;
    }
}
