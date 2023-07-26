using System;

public class EconomicManager : Singleton<EconomicManager>
{
    private int _money;
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
}
