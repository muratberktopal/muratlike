using UnityEngine;

public interface IRecruitable
{
    // void yerine bool yapýyoruz
    bool OnRecruit(Transform targetToFollow);

    bool IsRecruited { get; }
}