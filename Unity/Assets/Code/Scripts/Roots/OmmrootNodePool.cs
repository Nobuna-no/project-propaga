using NobunAtelier;

public class OmmrootNodePool : PoolManager
{
    public static OmmrootNodePool Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }
}
