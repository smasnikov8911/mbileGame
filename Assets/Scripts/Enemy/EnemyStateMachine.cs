using UnityEngine;

public class EnemyStateMachine
{
    // Текущее состояние врага
    public EnemyController.EnemyState CurrentState { get; private set; }
    
    // Предыдущее состояние врага (для возможного возврата)
    public EnemyController.EnemyState PreviousState { get; private set; }
    
    // Инициализация начального состояния
    public void Initialize(EnemyController.EnemyState startState)
    {
        CurrentState = startState;
        PreviousState = startState;
    }
    
    // Изменение состояния
    public void ChangeState(EnemyController.EnemyState newState)
    {
        // Сохраняем предыдущее состояние
        PreviousState = CurrentState;
        
        // Устанавливаем новое состояние
        CurrentState = newState;
        
        // Логирование изменения состояния (для отладки)
        Debug.Log($"Enemy state changed from {PreviousState} to {CurrentState}");
    }
    
    // Возврат к предыдущему состоянию
    public void RevertToPreviousState()
    {
        EnemyController.EnemyState tempState = CurrentState;
        CurrentState = PreviousState;
        PreviousState = tempState;
        
        Debug.Log($"Enemy reverted to previous state: {CurrentState}");
    }
}
