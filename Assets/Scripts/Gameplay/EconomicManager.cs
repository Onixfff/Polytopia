using System;

public class EconomicManager : Singleton<EconomicManager>
{
    private int _money;
    private int _aiMoney;
    private int _point;
    private int _aiPoint;
    public event Action OnMoneyChanged;
    public event Action OnPointChanged;
    
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
    
    public int Point
    {
        get => _point;
        private set
        {
            _point = value;
            OnPointChanged?.Invoke();
        }
    }
    
    public int AIPoint
    {
        get => _aiPoint;
        private set => _aiPoint = value;
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
    
    public void AddPoint(int count)
    {
        Point += count;
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
    
    public void AddAIPoint(int count)
    {
        AIPoint += count;
    }
}
