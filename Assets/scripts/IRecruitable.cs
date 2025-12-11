// IRecruitable.cs
using UnityEngine;

// Bu interface'i taþýyan her þey oyuncu tarafýndan toplanabilir demektir.
public interface IRecruitable
{
    // Komutan (oyuncu) bu birimi saflarýna kattýðýnda çalýþýr
    void OnRecruit(Transform targetToFollow);

    // Bu birim artýk takip modunda mý?
    bool IsRecruited { get; }
}
