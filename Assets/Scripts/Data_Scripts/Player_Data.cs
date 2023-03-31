[System.Serializable]
public class Player_Data
{
    public int money;

    public float[] playerPosition;

    public int scene;
   
    public Player_Data (PlayerInput playerInput)
    {
        playerPosition = new float[] { playerInput.transform.position.x, playerInput.transform.position.y, playerInput.transform.position.z };
    }
    
    public Player_Data (SceneVerification scene_Verification)
    {
        scene = scene_Verification.SceneIndex;
    }

    public Player_Data (PlayerController playerController)
    {
        money = playerController.Money;
    }
}