using System.Collections.Generic;
using UnityEngine;

namespace NullSave.TOCK.Inventory
{
    [HierarchyIcon("skinned_equip_point", false)]
    public class SkinnedEquipPoint : EquipPoint
    {

        #region Variables

        public SkinnedMeshRenderer boneSource;
        public SkinnedMeshRenderer defaultSkin;
        public Transform rigFallback;

        public List<string> boneSourceNames;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            drawGizmo = false;
        }

        #endregion

        #region Public Methods

        public override void EquipItem(InventoryItem item)
        {
            base.EquipItem(item);

            if (ObjectReference != null)
            {
                Transform[] allTrans = null;
                SkinnedMeshRenderer[] targets = ObjectReference.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer target in targets)
                {
                    if (target != null)
                    {
                        SkinnedBoneRemapper remapper = target.GetComponentInChildren<SkinnedBoneRemapper>();
                        bool boneMapped;

                        if (remapper != null)
                        {
                            Transform[] newBones = new Transform[remapper.boneNames.Count];

                            for (int i = 0; i < remapper.boneNames.Count; i++)
                            {
                                boneMapped = false;
                                foreach (Transform mapBone in boneSource.bones)
                                {
                                    if (mapBone.name == remapper.boneNames[i])
                                    {
                                        newBones[i] = mapBone;
                                        boneMapped = true;
                                        break;
                                    }
                                }

                                if (!boneMapped)
                                {
                                    if (allTrans == null && rigFallback != null)
                                    {
                                        allTrans = rigFallback.gameObject.GetComponentsInChildren<Transform>();
                                    }

                                    if (allTrans != null)
                                    {
                                        foreach (Transform trans in allTrans)
                                        {
                                            if (trans.name == remapper.boneNames[i])
                                            {
                                                boneMapped = true;
                                                newBones[i] = trans;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            target.bones = newBones;

                            boneMapped = false;
                            foreach (Transform mapBone in boneSource.bones)
                            {
                                if (mapBone.name == remapper.rootBone)
                                {
                                    target.rootBone = mapBone;
                                    boneMapped = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            target.bones = boneSource.bones;
                            target.rootBone = boneSource.rootBone;
                        }
                    }

                    if (defaultSkin != null)
                    {
                        defaultSkin.gameObject.SetActive(false);
                    }
                }
                if (Animator != null)
                {
                    Animator.Rebind();
                }
            }
        }

        public override void UnequipItem(int insertIndex = -1)
        {
            base.UnequipItem(insertIndex);

            // Restore default item as needed
            if (Item == null && defaultSkin != null)
            {
                defaultSkin.gameObject.SetActive(true);
            }
        }

        #endregion

    }
}