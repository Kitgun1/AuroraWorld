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
## Containers
```mermaid
flowchart BT
    GameplaySceneContainer --> GameContainer
    LobbySceneContainer --> GameContainer
```

## Modules
### Storage
Сохранение игровых данных проекта. Зарегестрирован в `GameContainer`, 
для получения -> `container.Resolve<Storage>();`<br>
В `Storage` есть методы:
* `Observable<T> Load<T>(string tag, T defaultObj = default)` - получение объекта. <br>
Чтобы получить сам объект, ипользуйте `Load<T>().Subscribe(T => ...);`
* `Observable<bool> Save<T>(string tag, T obj)` - запись объекта. <br>
**ОБЯЗАТЕЛЬНО:** `Save<T>().Subscribe(T => ...);` даже если не нужно проверять успешность операции.

### Resource
Позволяет немного оптимизировать процесс получения ресурсов из проекта
путем кеширования ресурсов определенного типа внутри себя. <br>
Как использовать:
```csharp
var resource = new Resource<T>();
var asset = resource.Load("asset path");
```
