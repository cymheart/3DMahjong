using DG.Tweening;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MahjongMachineNS
{
    public class SwapPaiHintArrowEffect
    {
        GameObject prefabSwapPaiHintOppositeArrow;
        GameObject prefabSwapPaiHintClockWiseArrow;
        GameObject prefabSwapPaiHintAntiClockWiseArrow;

        MahjongMachine mjMachine;
        MahjongAssets mjAssets;
        MahjongAssetsMgr mjAssetsMgr;
        Transform mjTable;

        GameObject swapPaiHintOppositeArrow;
        Transform swapPaiHintOppositeArrowUp;
        Transform swapPaiHintOppositeArrowDown;
        Tweener oppArrowUpTweener;
        Tweener oppArrowDownTweener;


        GameObject swapPaiHintClockWiseArrow;
        Tweener clockWiseTweener;

        GameObject swapPaiHintAntiClockWiseArrow;
        Tweener antiClockWiseTweener;

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssets = mjMachine.mjAssets;
            mjAssetsMgr = mjMachine.mjAssetsMgr;
            mjTable = mjMachine.mjtableTransform;

            prefabSwapPaiHintOppositeArrow = mjAssets.effectPrefabDict[(int)PrefabIdx.SWAPPAI_TIPS_OPP_ARROW][0];
            prefabSwapPaiHintClockWiseArrow = mjAssets.effectPrefabDict[(int)PrefabIdx.SWAPPAI_TIPS_CLOCKWISE_ARROW][0];
            prefabSwapPaiHintAntiClockWiseArrow = mjAssets.effectPrefabDict[(int)PrefabIdx.SWAPPAI_TIPS_ANTICLOCKWISE_ARROW][0];
        }

        public void Load()
        {
            LoadOppositeArrow();
            LoadClockWiseArrow();
            LoadAntiClockWiseArrow();
        }

        public void LoadOppositeArrow()
        {
            swapPaiHintOppositeArrow = Object.Instantiate(prefabSwapPaiHintOppositeArrow, mjTable);
            swapPaiHintOppositeArrow.SetActive(false);
            mjAssetsMgr.AppendToDestoryPool(swapPaiHintOppositeArrow);

            swapPaiHintOppositeArrowUp = swapPaiHintOppositeArrow.transform.Find("SwapPaiHintOppositeArrowUp");
            swapPaiHintOppositeArrowDown = swapPaiHintOppositeArrow.transform.Find("SwapPaiHintOppositeArrowDown");

            oppArrowUpTweener = swapPaiHintOppositeArrowUp.DOLocalMoveZ(-0.05f, 0.6f).SetRelative();
            oppArrowUpTweener.SetAutoKill(false);
            oppArrowUpTweener.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            oppArrowUpTweener.Pause();
            mjAssetsMgr.AppendToDestoryPool(oppArrowUpTweener);

            oppArrowDownTweener = swapPaiHintOppositeArrowDown.DOLocalMoveZ(0.05f, 0.6f).SetRelative();
            oppArrowDownTweener.SetAutoKill(false);
            oppArrowDownTweener.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            oppArrowDownTweener.Pause();
            mjAssetsMgr.AppendToDestoryPool(oppArrowDownTweener);
        }

        public void LoadClockWiseArrow()
        {
            swapPaiHintClockWiseArrow = Object.Instantiate(prefabSwapPaiHintClockWiseArrow, mjTable);
            swapPaiHintClockWiseArrow.SetActive(false);
            mjAssetsMgr.AppendToDestoryPool(swapPaiHintClockWiseArrow);

            clockWiseTweener = swapPaiHintClockWiseArrow.transform.DOLocalRotate(new Vector3(0, 20f, 0), 0.6f).SetRelative();
            clockWiseTweener.SetAutoKill(false);
            clockWiseTweener.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            clockWiseTweener.Pause();
            mjAssetsMgr.AppendToDestoryPool(clockWiseTweener);
        }

        public void LoadAntiClockWiseArrow()
        {
            swapPaiHintAntiClockWiseArrow = Object.Instantiate(prefabSwapPaiHintAntiClockWiseArrow, mjTable);
            swapPaiHintAntiClockWiseArrow.SetActive(false);
            mjAssetsMgr.AppendToDestoryPool(swapPaiHintAntiClockWiseArrow);

            antiClockWiseTweener = swapPaiHintAntiClockWiseArrow.transform.DOLocalRotate(new Vector3(0, 20f, 0), 0.6f).SetRelative();
            antiClockWiseTweener.SetAutoKill(false);
            antiClockWiseTweener.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            antiClockWiseTweener.Pause();
            mjAssetsMgr.AppendToDestoryPool(antiClockWiseTweener);
        }

        public void Destory()
        {
            Object.Destroy(swapPaiHintOppositeArrow);
            Object.Destroy(swapPaiHintClockWiseArrow);
            Object.Destroy(swapPaiHintAntiClockWiseArrow);

            oppArrowUpTweener.Kill();
            oppArrowDownTweener.Kill();
            clockWiseTweener.Kill();
            antiClockWiseTweener.Kill();
        }

        public void ShowArrow(SwapPaiDirection swapDir)
        {
            switch (swapDir)
            {
                case SwapPaiDirection.OPPOSITE:
                    ShowOppositeArrow();
                    break;

                case SwapPaiDirection.CLOCKWISE:
                    ShowClockWiseArrow();
                    break;

                case SwapPaiDirection.ANTICLOCKWISE:
                    ShowAnitClockWiseArrow();
                    break;
            }
        }

        public void HideArrow(SwapPaiDirection swapDir)
        {
            switch (swapDir)
            {
                case SwapPaiDirection.OPPOSITE:
                    HideOppositeArrow();
                    break;

                case SwapPaiDirection.CLOCKWISE:
                    HideClockWiseArrow();
                    break;

                case SwapPaiDirection.ANTICLOCKWISE:
                    HideAntiClockWiseArrow();
                    break;
            }
        }

        public void ShowOppositeArrow()
        {
            swapPaiHintOppositeArrow.gameObject.SetActive(true);
            oppArrowUpTweener.Play();
            oppArrowDownTweener.Play();
        }
        public void HideOppositeArrow()
        {
            swapPaiHintOppositeArrow.gameObject.SetActive(false);
            oppArrowUpTweener.Pause();
            oppArrowDownTweener.Pause();
        }

        public void ShowClockWiseArrow()
        {
            swapPaiHintClockWiseArrow.gameObject.SetActive(true);
            clockWiseTweener.Play();
            clockWiseTweener.Play();
        }
        public void HideClockWiseArrow()
        {
            swapPaiHintClockWiseArrow.gameObject.SetActive(false);
            clockWiseTweener.Pause();
            clockWiseTweener.Pause();
        }

        public void ShowAnitClockWiseArrow()
        {
            swapPaiHintAntiClockWiseArrow.gameObject.SetActive(true);
            antiClockWiseTweener.Play();
            antiClockWiseTweener.Play();
        }

        public void HideAntiClockWiseArrow()
        {
            swapPaiHintAntiClockWiseArrow.gameObject.SetActive(false);
            antiClockWiseTweener.Pause();
            antiClockWiseTweener.Pause();
        }
    }
}

