# Описание

Фреймворк для привязки данных к Gui для Unity 3D.

Основные возможности:

-   Привязка данных и коллекций к стандартным компонентам gui;
-   Привязка данных к полям и свойствам собственных компонентов;
-   Подписка на любые события UnityAction для всех элементов коллекции;
-   3 режима обновления данных: от источника к gui, от gui к источнику, двунаправленный;
-   Поддержка конвертеров;

# Примеры

В проекте, на сцене scene1.unity, реализован TodoMVC. Исходный код находится в директории GuiDataBindings/Assets/Scripts/Todos.

![Todos example](http://i.imgur.com/nrRZoU6.png])

# Подробно

### Компоненты привязки

Компонент привязки устанавлвает связь между полем источника данных (модели) и полем графического элемента.

Например GuiTextBind:

![GuiTextBind Component](http://i.imgur.com/yHSWoPo.png)

У каждого такого компонента обязательно есть 3 параметра:

**Bind Name** - свойство в источнике данных, к которому осуществляется привязка;

**Bind Way** - "направление" привязки.

Возможные значения:

-   _One Way_ - обновление только от источника к gui;
-   _From Gui To Source_ - обновление только от gui к источнику;
-   _Two Way_ - двунаправленная привязка;

    _From Gui To Source_ и _Two Way_ будет работать только на компонентах, которые подразумевают обновление источника данных (поля для ввода текста, чекбоксы, слайдеры и т.п.).

**Parameter Converter** - конвертер параметров, преобразовывает данные перед установкой их gui или источнику.

Стандартный конвертер типов:

![SimpleTypesConverter](http://i.imgur.com/Sld9vsF.png)

### Универсальный компонент привязки

GuiBindableUniversal - универсальный компонент привязки, позволяет указать любой компонент в качестве целевого, однако поддерживает только _One Way_ привязку.

**Bind Path** - публичное свойство или поле компонента, указанного в Bind Component. Поддерживаются вложенные свойства/поля, например, можно указать _MyProp.MyField_.

**Bind Component** - компонент к которому осуществляется привязка.

Например, здесь универсальный компонент привязки обновляет поле конвертера:

![](http://i.imgur.com/s4c3VyL.png)

### Компонент GuiComponent

Через этот компонент происходит привязка источника данных. Компонент, при создании (в методе Awake), начинает поиск компонентов привязки на текущем объекте и во всех дочерних рекурсивно.

Ниже импровизированный пример модели и модели представляния:

```csharp
public class PlayerStatusViewModel : MonoBehaviour
{
  [SerializeField]
  private GuiComponent _playerStatusGui;

  private void Start()
  {
    PlayerStatusData playerStatusData =
      GuiServices.
      GetService<IPlayerStatusDataService>().
      GetPlayerStatusData();

    // здесь осуществляется привязка
    _playerStatusGui.BindDataSource(playerStatusData);
  }
}

public class PlayerStatusData : ObservableObject
{
    private int _health;
    public int Health
    {
        get { return _health; }
        set { Set(() => Health, ref _health, value); }
    }

    private int _maxHealth;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { Set(() => MaxHealth, ref _maxHealth, value); }
    }
}
```

### Компонент GuiComponentsCollection

Через этот компонент происходит привязка коллекции данных, компонент имеет единственный параметр _Template_, где нужно указать префаб-шаблон, который будет создан для каждого элемента коллекции.

Сам префаб должен иметь компонент GuiComponent.

### Вместо команд

Обработчики событий можно установить, используя стандартный механизм Unity.

Однако такой подход не позволяет узнать на каком именно элементе коллекции было вызвано событие, поэтому вы можете из кода установить универсальный обработчик для событий Unity, который позволит идентифицировать элемент.

Пример кода из TodoMVC:

![GuiButtonBind](http://i.imgur.com/uWDGlRd.png)

```csharp
private void Start()
{
    ...

    _guiTasksList.AddEventListener("Remove", "onClick", (data, args) =>
    {
        _taskItems.RemoveAt(data.Index);
        UpdateCommonTasksInfo();
    });
}
```

Здесь аргумент data содержит поле Index, которое показывает на каком элементе коллекции произошло событие, а args это массив в котором содержатся аргументы, соответствующие параметрам события Unity.

_Примечание:_ Обработчик может быть установлен по имени компонента (Bind Name) и не имеет значения связан ли компонент с источником данных или нет.
