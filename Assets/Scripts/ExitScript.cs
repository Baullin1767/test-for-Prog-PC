using UniRx;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetKey(KeyCode.Escape))
            .Subscribe(_ =>
            {
                Application.Quit();
            })
            .AddTo(this);
    }
}
