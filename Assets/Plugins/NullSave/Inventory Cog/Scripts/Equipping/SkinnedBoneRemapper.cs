using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class SkinnedBoneRemapper : MonoBehaviour
    {

        #region Variables

        public string rootBone;
        public List<string> boneNames;

        #endregion

    }
}