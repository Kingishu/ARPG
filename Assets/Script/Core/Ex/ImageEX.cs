using UnityEngine;
using UnityEngine.UI;

public static class ImageEX
{
    public static bool SetFillAmount(this Image image,float v,float speed)
    {
        if (image.fillAmount>v)
        {
            float temp=image.fillAmount-GameTime.deltaTime*speed;
            if (temp<v)
            {
                temp = v;
            }
            image.fillAmount=temp;
        }
        else if (image.fillAmount<v)
        {
            float temp=image.fillAmount+GameTime.deltaTime*speed;
            if (temp>v)
            {
                temp = v;
            }
            image.fillAmount=temp;
        }
        //浮点数相等比较有风险,用Approximately代替
        //等同于 return image.fillAmount==v;
        return Mathf.Approximately(image.fillAmount, v);
    }
}
