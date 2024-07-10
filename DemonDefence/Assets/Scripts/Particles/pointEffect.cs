using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class pointEffect : MonoBehaviour
{
    /// Functionality for effects that need to be fired at particular times, but do not need to be constant.
    abstract public void fireEffect(); /// Method to fire off any effects
}
