# AuroraWorld

## Архитектура проекта

#### Архитектура сцен
```mermaid
flowchart TB
    app[Запуск приложения]
        
    gameplay[Геймплей]
    gameplay_enter_params{{"Входные параметры (Геймплей)"}}
    gameplay_exit_params{{"Выходны параметры (Геймплей)"}}
        
    lobby[Лобби]
    lobby_enter_params{{"Входные параметры (Лобби)"}}
    lobby_exit_params{{"Выходны параметры (Лобби)"}}
        
    other_scene["Другая сцена"]
    other_scene_enter_params{{"Входные параметры (Другой сцены)"}}
    app --> lobby
    lobby --> lobby_exit_params
    lobby_exit_params -.-> gameplay_enter_params
    lobby_exit_params -.-> other_scene_enter_params
        
    gameplay --> gameplay_exit_params
    gameplay_exit_params -.-> other_scene_enter_params
    gameplay_exit_params -.-> lobby_enter_params
        
    gameplay_enter_params --> gameplay
    lobby_enter_params --> lobby
    other_scene_enter_params --> other_scene
```


## Modules

### Storage

Storage - модуль для сохранения результатов игры.<br>
Примеры использования:

```csharp
// Регистрация модуля
container.RegisterSingleton(_ => new Storage());

// Получение модуля
var storage = container.Resolve<Storage>();

// Сохранение объекта
storage.Save(tag, obj);

// Получение объекта
var obj = storage.Load(tag, defaultObj);
```

### Resource

Resource - модуль для получения объектов из ресурсов<br>
Примеры использования:

```csharp
// Регистрация модуля
container.RegisterSingleton("name", _ => new Resource<T>()); // where T : Object

// Получение модуля
var resource = сontainer.Resolve<Resource>();

// Получение ресурса
T obj = resource.Load("path");
```