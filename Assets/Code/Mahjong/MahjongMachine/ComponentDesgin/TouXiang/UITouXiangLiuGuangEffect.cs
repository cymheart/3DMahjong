using CoreDesgin;
using UnityEngine;
using UnityEngine.UI;

namespace ComponentDesgin
{
    public class UITouXiangLiuGuangEffect
    {
        private static int _alpha, _color, _angle, _centerx, _centery, _startAng;
        static float halfPI = Mathf.PI / 2;


        MahjongMachine mjMachine;
        MahjongAssetsMgr mjAssetsMgr;
        GameObject touXiangEffectGo;
        Transform touXiangTransform;
        GameObject headParticle = null;

        float startAng = 0;
        Material mat;

        public float duration = 10;
        float startTime;
        float lastTime;

        float startAlpha = 0.6f;
        float endAlpha = 2.5f;
        float perAlpha = 0.04f;
        float curtAlpha;


        float startColor = 0.6f;
        float endColor = 0.99f;
        float perColor = 0.005f;
        Color curtColor = new Color(1, 0.9f, 0.9f, 1);

        float[] angRange = new float[9];

        Vector2 center;
        Vector2[] pt = new Vector2[4];
        float scaleDistance = 15;
        Vector2 scale = new Vector2(1, 1);
        Vector2 orgSize;

        bool isStop = false;

        static UITouXiangLiuGuangEffect()
        {
            _centerx = Shader.PropertyToID("_CenterX");
            _centery = Shader.PropertyToID("_CenterY");
            _startAng = Shader.PropertyToID("_StartAng");

            _alpha = Shader.PropertyToID("_Alpha");
            _color = Shader.PropertyToID("_Color");
            _angle = Shader.PropertyToID("_Angle");
        }

        public void Setting(MahjongMachine mjMachine, GameObject touXiangEffectGo)
        {
            this.mjMachine = mjMachine;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            this.touXiangEffectGo = touXiangEffectGo;

            touXiangTransform = touXiangEffectGo.transform;

            Image img = touXiangEffectGo.GetComponent<Image>();
            mat = img.material;
            Vector2[] uvs = img.sprite.uv;
            Vector2 centerm = (uvs[0] + uvs[3]) / 2;
            mat.SetFloat(_centerx, centerm.x);
            mat.SetFloat(_centery, centerm.y);

            mat.SetFloat(_alpha, 1f);
            curtAlpha = 1f;

            startAng = mat.GetFloat(_startAng);

            lastTime = startTime = Time.time;

            RectTransform rectTransform = touXiangEffectGo.GetComponent<RectTransform>();
            center = rectTransform.localPosition;
            float x = rectTransform.sizeDelta.x * rectTransform.localScale.x;
            float y = rectTransform.sizeDelta.y * rectTransform.localScale.y;

            orgSize = new Vector2(img.sprite.textureRect.width, img.sprite.textureRect.height);
            scale.x = x / orgSize.x;
            scale.y = y / orgSize.y;

            orgSize.x -= scaleDistance;
            orgSize.y -= scaleDistance;

            float halfWidth = orgSize.x / 2;
            float halfHeight = orgSize.y / 2;

            pt[0] = new Vector2(halfWidth, halfHeight);
            pt[1] = new Vector2(-halfWidth, halfHeight);
            pt[2] = new Vector2(-halfWidth, -halfHeight);
            pt[3] = new Vector2(halfWidth, -halfHeight);


            float n = Mathf.Atan2(halfHeight, halfWidth);
            float m = halfPI - n;

            angRange[3] = -n;
            angRange[2] = angRange[3] - m * 2;
            angRange[1] = angRange[2] - n * 2;
            angRange[0] = angRange[1] - m * 2;

            angRange[4] = angRange[3] + n * 2;
            angRange[5] = angRange[4] + m * 2;
            angRange[6] = angRange[5] + n * 2;
            angRange[7] = angRange[6] + m * 2;
            angRange[8] = angRange[7] + n * 2;

            Stop();
        }

        public float GetTotalTime()
        {
            return Time.time - startTime;
        }

        public void Stop()
        {
            if (isStop)
                return;

            touXiangEffectGo.SetActive(false);
            isStop = true;

            if (headParticle != null)
            {
                headParticle.GetComponent<ParticleSystem>().Stop();
                mjAssetsMgr.touxiangHeadParticlePool.PushGameObject(headParticle);
                headParticle = null;
            }
        }

        public void Run()
        {
            if (isStop == false)
                return;

            startTime = Time.time;
            headParticle = mjAssetsMgr.touxiangHeadParticlePool.PopGameObject();

            if (headParticle.transform.parent != touXiangTransform.parent)
            {
                headParticle.transform.SetParent(touXiangTransform.parent);
            }

            headParticle.SetActive(true);
            headParticle.GetComponent<ParticleSystem>().Play();
            touXiangEffectGo.SetActive(true);
            isStop = false;
        }

        public void Update()
        {
            if (isStop)
                return;

            float curtTime = Time.time;

            //angle
            float tm = Common.Mod(curtTime - startTime, duration);
            float curtAng = tm / duration * 360.0f;
            mat.SetFloat(_angle, curtAng);

            //alpha
            curtAlpha += perAlpha;
            if (curtAlpha >= endAlpha || curtAlpha <= startAlpha)
                perAlpha = -perAlpha;
            mat.SetFloat(_alpha, curtAlpha);

            //color
            curtColor.g += perColor;
            curtColor.b += perColor;
            if (curtColor.g >= endColor || curtColor.g <= startColor)
                perColor = -perColor;

            mat.SetColor(_color, curtColor);


            Vector2 m = ComputedPos(curtAng);
            headParticle.transform.localPosition = new Vector3(m.x, m.y, headParticle.transform.localPosition.z);

            if (curtTime - lastTime < 0.12f)
                return;

            lastTime = curtTime;

            GameObject touxiangParticle = mjAssetsMgr.touxiangParticlePool.PopGameObject();
            touxiangParticle.SetActive(true);

            if (touxiangParticle.transform.parent != touXiangTransform.parent)
            {
                touxiangParticle.transform.SetParent(touXiangTransform.parent);
            }

            touxiangParticle.transform.localPosition = new Vector3(m.x, m.y, touxiangParticle.transform.localPosition.z);
            touxiangParticle.GetComponent<ParticleSystem>().Play();

        }


        Vector2 ComputedPos(float ang)
        {
            ang += startAng;
            float nAng = ang;

            if (nAng > 180f) { nAng = 360f - nAng; }
            else if (nAng > 0f) { nAng = 180f - nAng; }
            else if (nAng > -180f) { nAng = -nAng; }
            else if (nAng > -360f) { nAng = -(nAng + 180f); }


            float rad = Mathf.PI * ang / 180f;
            float lineRad = Mathf.PI * nAng / 180f;

            Vector2 pos = new Vector2(0, 0);

            for (int i = 0; i < 8; i++)
            {
                if (rad > angRange[i] && rad <= angRange[i + 1])
                {
                    pos = _ComputedPos(i, lineRad);
                    break;
                }
            }

            return pos;
        }

        Vector2 _ComputedPos(int i, float lineRad)
        {
            float x = 0, y = 0;
            Vector2 pos;

            switch (i)
            {
                case 0:
                case 4:
                    {
                        if (lineRad > halfPI - 0.001f && lineRad < halfPI + 0.001f)
                        {
                            x = 0;
                            y = pt[2].y;
                            break;
                        }

                        float n = Mathf.Tan(lineRad);
                        x = pt[2].y / n;
                        y = pt[2].y;
                    }

                    break;

                case 1:
                case 5:
                    {
                        float n = Mathf.Tan(lineRad);
                        x = pt[2].x;
                        y = pt[2].x * n;
                    }
                    break;

                case 2:
                case 6:
                    {
                        if (lineRad > halfPI - 0.001f && lineRad < halfPI + 0.001f)
                        {
                            x = 0;
                            y = pt[1].y;
                            break;
                        }

                        float n = Mathf.Tan(lineRad);
                        x = pt[1].y / n;
                        y = pt[1].y;
                    }
                    break;

                case 3:
                case 7:
                    {
                        float n = Mathf.Tan(lineRad);
                        x = pt[0].x;
                        y = pt[0].x * n;
                    }
                    break;
            }

            pos = new Vector2(x * scale.x + center.x, y * scale.y + center.y);
            return pos;

        }

    }
}