using System;

public class EconomicManager : Singleton<EconomicManager>
{
    private int _money;
    private int _aiMoney;
    public event Action OnMoneyChanged;
    
    public int Money
    {
        get => _money;
        private set
        {
            _money = value;
            OnMoneyChanged?.Invoke();
        }
    }
    
    public int AIMoney
    {
        get => _aiMoney;
        private set => _aiMoney = value;
    }
    
    public void BuySomething(int cost)
    {
        Money -= cost;
    }

    public bool IsCanBuy(int cost)
    {
        return Money >= cost;
    }

    public void AddMoney(int count)
    {
        Money += count;
    }
    
    public bool IsAICanBuy(int cost)
    {
        return AIMoney >= cost;
    }
    
    public void AIBuySomething(int cost)
    {
        AIMoney -= cost;
    }
    
    public void AddAIMoney(int count)
    {
        AIMoney += count;
    }
}
