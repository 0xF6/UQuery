<!-- Name -->
<h1 align="center">
  🔥 Unity Query 🔥
</h1>
<!-- desc -->
<h4 align="center">
  Library for querying unity gameobject
</h4>

<!-- classic badges -->
<p align="center">
  <a href="#">
    <img src="http://img.shields.io/:license-MIT-blue.svg">
  </a>
  <a href="https://github.com/0xF6/UQuery/releases">
    <img src="https://img.shields.io/github/release/0xF6/UQuery.svg?logo=github&style=flat">
  </a>
</p>

<!-- popup badges -->
<p align="center">
   <a href="https://openupm.com/packages/com.ivysola.uquery/">
    <img alt="OpenUPM" src="https://img.shields.io/npm/v/com.ivysola.uquery?label=openupm&registry_uri=https://package.openupm.com">
  </a>
  <a href="https://t.me/ivysola">
    <img src="https://img.shields.io/badge/Ask%20Me-Anything-1f425f.svg?style=popout-square&logo=telegram">
  </a>
</p>

<!-- big badges -->
<p align="center">
  <a href="#">
    <img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg">
    <img src="https://forthebadge.com/images/badges/designed-in-ms-paint.svg">
    <img src="https://forthebadge.com/images/badges/ages-18.svg">
    <img src="https://ForTheBadge.com/images/badges/winter-is-coming.svg">
    <img src="https://forthebadge.com/images/badges/gluten-free.svg">
  </a>
</p>


## 📌 Usage


```csharp
/* graph of game objects
  %scene_root% |
        - Canvas |
            - Layout |
                - Header |
                    - Title | <UIText>
*/

var result = UQuery.SelectByPath<UIText>("Canvas>Layout>Header>Title[UIText]");
result // UIText component

var result = UQuery.SelectByPath<GameObject>("Canvas>Layout>ButtonGroup");
result // GameObject 'ButtonGroup'

/*
%root% |
    - Canvas |
        - Layout |
            - Header |
                - Title1
            - Header |
                - Title2
            - Header |
                - Title3
            - Header |
                - Title4
*/

var result = UQuery.SelectByPath<UIText>("Canvas>Layout>Header:(2)>Title3");
result // GameObject 'Title3'
```


## 🧬 Roadmap

- [x] Light Documentation
- [ ] Samples
- [ ] Validate IL2CPP Target
- [x] Directives for MORELINQ & UNITY_LINQ
- [ ] Support querying by tag\layout (?)
- [ ] Aliases in Query Path
- [ ] Strong validation path format
- [x] Additional exceptions types
- [ ] Query cache system 
- [x] Access child (same names) by index
