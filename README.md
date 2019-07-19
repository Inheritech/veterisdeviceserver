# Veteris Device Server

## Español

Este es un servidor de IoT simple hecho en su totalidad con C#, tiene muchos puntos de mejora y para ejecutar hara falta una limpieza profunda, así como remplazar librerias.

El servidor requiere una implementación de WatsonTCP como servidor para canales TCP, el NuGet servira.
También, este servidor se comunicaba de forma poco segura y mal implementada con una nube de datos, por lo que requiere de remover toda referencia de la nube.
(O comentar e implementar después)

Por ahora, no hay extra documentación aunque los pull request son bienvenidos.

Si creas algo interesante con esto o te ayuda de cualquier manera, por favor hazlo saber, seria genial ver que alguien logre algo con este código.

## English

This is a simple IoT edge server made completely in C#, it has many sites that require improvement and just to build it will need some cleaning and some libraries need to be replaced.
Also, this server had a badly implemented and insecure communication channel with a data cloud, so you will need to remove every reference to that cloud (Or comment the code and reimplement it later).
Almost every comment and log trace is written in spanish so there's that.

Right now there isn't too much of a documentation document but pull requests are welcome.

If you create something interesting or this helps you in any way, please let me know, it would be great seeing someone accomplish something with this code.

## Help on setting up

For any inquiry or support need, you can leave an issue.

## LICENSE

The MIT License

Copyright 2019 Leopoldo Berumen Juárez

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.