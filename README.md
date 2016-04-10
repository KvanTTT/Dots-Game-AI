## Dots-Game-AI

Yet another implementation of [Dots Game](https://en.wikipedia.org/wiki/Dots_(game)) Artificial Intelligence (with russian rules).

### Features

* Save to & load from [SGF](https://en.wikipedia.org/wiki/Smart_Game_Format) and [PointsXT](http://pointsgame.net/site/pointsxt) files.
* Working with game trees.
* Interaction with [Sport Dots vk.com](https://vk.com/app4214777_1194928) application.

### Unit Tests
[![Build Status](https://travis-ci.org/KvanTTT/Dots-Game-AI.svg?branch=master)](https://travis-ci.org/KvanTTT/Dots-Game-AI)

### Roadmap

* Collect statistics for open games with strong players from playdots.ru.
* Train neural network with statistics info. Use [CNTK](https://github.com/Microsoft/CNTK) or another deep-learning framwork.

## Screen

![DotsGameAI](https://habrastorage.org/files/b19/3b5/5d7/b193b55d722d414985b7c3450bac214a.png)

### Usefull links

* [opai-rs](https://github.com/kurnevsky/opai-rs). Opai-rs an artificial intelligence for the game of points.
* [AlphaGo Replication](https://github.com/Rochester-NRT/AlphaGo). This project is a replication/reference implementation of DeepMind's 2016 Nature publication
* [Michi](https://github.com/pasky/michi). Minimalistic Go MCTS Engine.
* [patchi](https://github.com/pasky/pachi). A fairly strong Go/Baduk/Weiqi playing program.

### Dependencies

* [Perspex](https://github.com/Perspex/Perspex); a multi-platform .NET UI framework;
* [Json.NET](http://www.newtonsoft.com/json) for serialization and deserialization to JSON;
* [NUnit 3](http://www.nunit.org/) for unit tests.

### License

Dots Game AI is licensed under the Apache 2.0 License.