[System.Serializable]
public class Player_Data
{
    public int money;

    public float[] playerPosition;

    public int scene;

    public int LevelEnclo1;
    public int LevelEnclo2;
    public int LevelEnclo3;
   
    public Player_Data (playerInput playerInput)
    {
        playerPosition = new float[] { playerInput.transform.position.x, playerInput.transform.position.y, playerInput.transform.position.z };
    }
    
    public Player_Data (Scene_verification scene_Verification)
    {
        scene = scene_Verification.sceneIndex;
    }
    public Player_Data (Shop_Enclos shop_Enclos)
    {
        LevelEnclo1 = shop_Enclos.levelEnclo1;
        LevelEnclo2 = shop_Enclos.levelEnclo2;
        LevelEnclo3 = shop_Enclos.levelEnclo3;
    }

    public Player_Data (playerController playerController)
    {
        money = playerController.money;
    }
}
