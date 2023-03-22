using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private List<GameObject> shopsUI;

    private PlayerController playerController;
    private PlayerInput playerInput;
    private List_Slots listSlots;

    private GameObject currentShopUI = null;
    private string interaction;

    [Header("shop states")]
    [SerializeField] private bool canUseShop = false;
    [SerializeField] private bool shopInUse = false;
    [SerializeField] private ShopRole shopRole;

    public bool CanUseShop
    {
        get { return canUseShop; }
        set { canUseShop = value; }
    }

    [Header("shop Items Dealer")]
    [SerializeField] private List<ItemToBuy> itemsToBuy;
    [SerializeField] private List<RecipeToBuy> recipesToBuy;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject recipesPanel;
    [SerializeField] private Transform recipesParent;
    [SerializeField] private GameObject itemToBuyUI;

    [Serializable]
    public class ItemToBuy
    {
        public Item item;
        public int price;
    }

    [Serializable]
    public class RecipeToBuy
    {
        public Recipe item;
        public int price;
    }

    public enum ShopRole
    { 
        FruitSeller,
        ItemDealer,
        AnimalPenManager
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerInput = FindObjectOfType<PlayerInput>();
        listSlots = FindObjectOfType<List_Slots>();

        interaction = "Use " + playerInput.InteractionAction.GetBindingDisplayString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            listSlots.UpdateMoney(100);
        }

        HandleShopUse();

        HandleShopUI();
    }

    #region Shop Use

    private void HandleShopUse()
    {
        if (canUseShop)
        {
            interactionUI.SetActive(true);
        }
        else
        {
            interactionUI.SetActive(false);
        }
    }

    #endregion

    #region Shop UI

    private void HandleShopUI()
    {
        if (currentShopUI != null)
            currentShopUI.SetActive(shopInUse);

        if (!canUseShop)
        {
            CloseShopUI();
            return;
        }

        if (playerInput.InteractionAction.triggered && canUseShop)
        {
            if (!shopInUse)
            {
                OpenShopUI();
            }
            else
            {
                CloseShopUI();
            }
        }
    }

    private GameObject GetCurrentShopUI()
    {
        GameObject shopUI = null;

        for (int i = 0; i < shopsUI.Count; i++)
        {
            if (shopsUI[i].name.Contains(shopRole.ToString()))
            {
                shopUI = shopsUI[i];
            }
        }

        return shopUI;
    }

    private void OpenShopUI()
    {
        currentShopUI = GetCurrentShopUI();

        shopInUse = true;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to close shop";

        MinigameManager.AddOpenInventory(currentShopUI);
    }

    private void CloseShopUI()
    {
        shopInUse = false;

        interactionUI.GetComponentInChildren<Text>().text = $"{interaction} to open shop";

        MinigameManager.RemoveOpenInventory(currentShopUI);

        Clear();
    }

    #endregion

    #region Shop Content

    #region Shop "Fruit Seller"

    private void HandleFruitSeller()
    {
        Debug.Log($"Fruit Seller");
    }

    #endregion

    #region Shop "Item Dealer"

    public void ShowItems()
    {
        if (itemsToBuy == null) return;

        Clear();

        itemsPanel.SetActive(true);
        recipesPanel.SetActive(false);

        itemsParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < itemsToBuy.Count; i++)
        {
            Transform item = Instantiate(itemToBuyUI, itemsParent).transform;

            ItemToBuy itemToBuy = itemsToBuy[i];

            item.GetChild(0).GetComponent<Image>().sprite = itemToBuy.item.itemSprite;
            item.GetChild(1).GetComponent<Text>().text = itemToBuy.item.itemName;
            item.GetChild(2).GetComponent<Text>().text = $"{itemToBuy.price} P";

            InputField inputField = item.GetChild(3).GetComponent<InputField>();
            inputField.text = $"{0}";
            item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyItem(itemToBuy, inputField); });
        }
    }

    public void ShowRecipes()
    {
        if (recipesToBuy == null) return;

        Clear();

        itemsPanel.SetActive(false);
        recipesPanel.SetActive(true);

        recipesParent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < recipesToBuy.Count; i++)
        {
            Transform item = Instantiate(itemToBuyUI, recipesParent).transform;

            RecipeToBuy recipeToBuy = recipesToBuy[i];

            item.GetChild(0).GetComponent<Image>().sprite = recipeToBuy.item.recipeSprite;
            item.GetChild(1).GetComponent<Text>().text = recipeToBuy.item.recipeName;
            item.GetChild(2).GetComponent<Text>().text = $"{recipeToBuy.price} P";

            InputField inputField = item.GetChild(3).GetComponent<InputField>();
            inputField.text = $"{0}";
            item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyRecipe(recipeToBuy, inputField); });
        }
    }

    private void BuyItem(ItemToBuy itemToBuy, InputField inputField)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        if (playerController.Money >= (amount * itemToBuy.price))
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject itemUI = listSlots.PlayerSlotsParent.GetComponent<PlayerInventory>().CreateItemUI();

                itemUI.GetComponent<DraggableItem>().quantityStacked = 1;

                itemUI.GetComponent<DraggableItem>().Item = itemToBuy.item;

                listSlots.UpdateMoney(playerController.Money - itemToBuy.price);
            }
        }
    }

    private void BuyRecipe(RecipeToBuy recipeToBuy, InputField inputField)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        if (playerController.Money >= (amount * recipeToBuy.price))
        {
            for (int i = 0; i < amount; i++)
            {
                listSlots.PRInventory.AddRecipeToInventory(recipeToBuy.item);

                listSlots.UpdateMoney(playerController.Money - recipeToBuy.price);
            }
        }
    }

    private void Clear()
    {
        itemsPanel.SetActive(false);
        recipesPanel.SetActive(false);

        for (int i = 0; i < itemsParent.childCount; i++)
        {
            Destroy(itemsParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < recipesParent.childCount; i++)
        {
            Destroy(recipesParent.GetChild(i).gameObject);
        }
    }

    #endregion

    #region Shop "Animal Pen Manager"

    private void HandleAnimalPenManager()
    {
        Debug.Log($"Animal Pen Manager");
    }

    #endregion

    #endregion
}