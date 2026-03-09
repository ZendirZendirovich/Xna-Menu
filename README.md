![License: MIT](https://img.shields.io)

# Xna-Menu
Простенькое меню на MonoGame/XNA

есть почти все важные функции, игра, настройки и выйти,
есть красивая анимация узора, изменение спрайтов кнопок при выделении, и шрифт майнкрафт.
используется .NET 9.0,
Настройки очень хорошо проделаны, всё сохраняется в (AppData\Local\\*Название игры*)

<img width="1196" height="70" alt="image" src="https://github.com/user-attachments/assets/33ff3a11-1314-4f0f-920c-f21bee3a7a32" />

в коде (SaveSystem.cs) можно поменять название папки (первая строчка)
и название + расширение файла (во второй строчке)

*расширение должно быть таким, чтобы его смог открыть текстовый редактор (txt, json и тд)*

<img width="792" height="46" alt="image" src="https://github.com/user-attachments/assets/ea29fb66-3ae2-4cb8-9ef7-378befdba13c" />



меню создано при помощи официального обучнения MonoGame(если это именно тот сайт конечно):
https://docs.monogame.net/articles/tutorials/building_2d_games/17_scenes/index.html

# Скриншоты
<img width="1292" height="765" alt="image" src="https://github.com/user-attachments/assets/0c5ef675-b20c-4d61-9510-6dd3515e06cd" />

<img width="1289" height="765" alt="image" src="https://github.com/user-attachments/assets/8a73d9ec-9d9a-4f1b-9a1b-3fbf094856b1" />

# Частые проблемы (чаще всего вина самого движка)

**Проект при открытии создаёт куча ошибок и не запускается:**

Частая проблема самого движка, чтобы исправить просто создайте новый CrossPlatform MonoGame проект и перекиньте код, со всеми спрайтами из папки Content через MGCB

**Шрифт Minecraft Rus Regular.ttf нужно просто закинуть через VS(либо другая программа через которую вы делаете)в папку Content, НЕ ЧЕРЕЗ MGCB!!**

## License
Этот проект распространяется под лицензией **MIT**. 
Вы можете свободно использовать, копировать и изменять код, при условии, что в вашем проекте будет **указана ссылка на оригинал и сохранено имя автора**.
