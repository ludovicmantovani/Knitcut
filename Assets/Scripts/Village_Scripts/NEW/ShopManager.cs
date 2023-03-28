using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject itemToBuyUI;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private List<GameObject> shopsUI;

    public GameObject InteractionUI
    {
        get { return interactionUI; }
        set { interactionUI = value; }
    }

    private PlayerController playerController;
    private PlayerInput playerInput;
    private List_Slots listSlots;

    private GameObject currentShopUI = null;
    private string interaction;

    [Header("Shop States")]
    [SerializeField] private bool canUseShop = false;
    [SerializeField] private bool shopInUse = false;
    [SerializeField] private ShopRole shopRole;

    public bool CanUseShop
    {
        get { return canUseShop; }
        set { canUseShop = value; }
    }

    [Header("Shop Fruit Seeds Seller")]
    [SerializeField] private List<ItemToBuy> fruitSeedsToBuy;
    [SerializeField] private GameObject fruitSeedsPanel;
    [SerializeField] private Transform fruitSeedsParent;

    [Header("Shop Items Dealer")]
    [SerializeField] private List<ItemToBuy> itemsToBuy;
    [SerializeField] private List<RecipeToBuy> recipesToBuy;
    [SerializeField] private GameObject itemsPanel;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject recipesPanel;
    [SerializeField] private Transform recipesParent;

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
        FruitSeedsSeller,
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
            listSlots.UpdateMoney(1000);
        }

        HandleShopUI();
    }

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

    #region Shop "Fruit Seeds Seller"

    public void ShowSeeds()
    {
        Show(fruitSeedsToBuy, fruitSeedsPanel, fruitSeedsParent);
    }

    #endregion

    #region Shop "Item Dealer"

    public void ShowItems()
    {
        Show(itemsToBuy, itemsPanel, itemsParent);
    }

    public void ShowRecipes()
    {
        Show(recipesToBuy, recipesPanel, recipesParent, true);
    }

    private void BuyItem(ItemToBuy itemToBuy, InputField inputField)
    {
        int amount = int.Parse(inputField.text);

        if (amount == 0) return;

        if (playerController.Money >= (amount * itemToBuy.price))
        {
            if (amount <= itemToBuy.item.maxStackSize)
            {
                CreateItemUI(itemToBuy, amount);
            }
            else
            {
                int nbItemsToCreate = amount / itemToBuy.item.maxStackSize;

                Debug.Log($"{amount / itemToBuy.item.maxStackSize}");
                Debug.Log($"{amount % itemToBuy.item.maxStackSize}");
                Debug.Log($"{amount - (nbItemsToCreate * itemToBuy.item.maxStackSize)}");

                for (int i = 0; i < nbItemsToCreate; i++)
                {
                    CreateItemUI(itemToBuy, itemToBuy.item.maxStackSize);
                }

                int lastQuantity = amount - (nbItemsToCreate * itemToBuy.item.maxStackSize);

                if (lastQuantity == 0) return;

                CreateItemUI(itemToBuy, lastQuantity);
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

    #endregion

    #region Shop "Animal Pen Manager"

    private void HandleAnimalPenManager()
    {
        Debug.Log($"Animal Pen Manager");
    }

    #endregion

    private void Show(object objectsToBuy, GameObject panel, Transform parent, bool isRecipe = false)
    {
        IList listObjectsToBuy = (IList)objectsToBuy;

        if (listObjectsToBuy == null) return;

        Clear();

        panel.SetActive(true);

        parent.GetComponent<Swipe>().ScrollPos = 1;

        for (int i = 0; i < listObjectsToBuy.Count; i++)
        {
            ShowObjectUI(parent, listObjectsToBuy[i], isRecipe);
        }
    }

    private void ShowObjectUI(Transform parent, object objectToBuy, bool isRecipe)
    {
        Transform item = Instantiate(itemToBuyUI, parent).transform;

        Sprite objectSprite;
        string objectName;
        float objectPrice;

        if (!isRecipe)
        {
            ItemToBuy itemToBuy = (ItemToBuy)objectToBuy;

            objectSprite = itemToBuy.item.itemSprite;
            objectName = itemToBuy.item.itemName;
            objectPrice = itemToBuy.price;
        }
        else
        {
            RecipeToBuy recipeToBuy = (RecipeToBuy)objectToBuy;

            objectSprite = recipeToBuy.item.recipeSprite;
            objectName = recipeToBuy.item.recipeName;
            objectPrice = recipeToBuy.price;
        }

        item.GetChild(0).GetComponent<Image>().sprite = objectSprite;
        item.GetChild(1).GetComponent<Text>().text = objectName;
        item.GetChild(2).GetComponent<Text>().text = $"{objectPrice} P";

        InputField inputField = item.GetChild(3).GetComponent<InputField>();
        inputField.text = $"{0}";

        if (!isRecipe)
        {
            item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyItem((ItemToBuy)objectToBuy, inputField); });
        }
        else
        {
            item.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { BuyRecipe((RecipeToBuy)objectToBuy, inputField); });
        }
    }

    private void Clear()
    {
        if (itemsPanel != null)
        {
            itemsPanel.SetActive(false);

            for (int i = 0; i < itemsParent.childCount; i++)
            {
                Destroy(itemsParent.GetChild(i).gameObject);
            }
        }

        if (recipesPanel != null)
        {
            recipesPanel.SetActive(false);

            for (int i = 0; i < recipesParent.childCount; i++)
            {
                Destroy(recipesParent.GetChild(i).gameObject);
            }
        }

        if (fruitSeedsPanel != null)
        {
            fruitSeedsPanel.SetActive(false);

            for (int i = 0; i < fruitSeedsParent.childCount; i++)
            {
                Destroy(fruitSeedsParent.GetChild(i).gameObject);
            }
        }
    }

    private void CreateItemUI(ItemToBuy itemToBuy, int quantity)
    {
        GameObject itemUI = listSlots.PlayerSlotsParent.GetComponent<PlayerInventory>().CreateItemUI();

        itemUI.GetComponent<DraggableItem>().quantityStacked = quantity;

        itemUI.GetComponent<DraggableItem>().Item = itemToBuy.item;

        listSlots.UpdateMoney(playerController.Money - itemToBuy.price);
    }

    #endregion
}