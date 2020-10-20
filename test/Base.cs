namespace test
{
    using UnityEngine;
    using UnityEngine.Query;
    using Xunit;
    using static UnityEngine.Query.UQuery;

    public class Base
    {
        [Fact]
        public void Test00()
        {
            void SetupTest00()
            {
                /*
                %root% |
                    - Canvas |
                        - Layout |
                            - Header |
                                - Title | <UIText>
                 */
                var canvas = new GameObject("Canvas");
                var layout = new GameObject("Layout");
                var header = new GameObject("Header");
                var title = new GameObject("Title");
                var uiText = new UIText();


                title.components.Add(uiText);
                header.transform.Childs.Add(title);
                layout.transform.Childs.Add(header);
                canvas.transform.Childs.Add(layout);

                __GO__.Add(canvas);
            }

            SetupTest00();

            var result = SelectByPath<UIText>("Canvas>Layout>Header>Title[UIText]");

            Assert.NotNull(result);


            __GO__.goStorage.Clear();
        }
        [Fact]
        public void Test01()
        {
            void SetupTest01()
            {
                /*
                %root% |
                    - X |
                     - D |
                      - Z |
                 */
                var X = new GameObject("X");
                var D = new GameObject("D");
                var Z = new GameObject("Z");

                D.transform.Childs.Add(Z);
                X.transform.Childs.Add(D);

                __GO__.Add(X);
            }

            SetupTest01();
            Assert.Throws<IncorrectPathException>(() => SelectByPath<UIText>(""));
            Assert.Throws<IncorrectPathException>(() => SelectByPath<UIText>(null));
            Assert.Throws<QueryTypeMismatchException>(() => SelectByPath<UIText>("T>D>X"));
            Assert.Throws<GameObjectNotFoundByPath>(() => SelectByPath<GameObject>("N>S>A"));
            Assert.Throws<GameObjectNotFoundByPath>(() => SelectByPath<GameObject>("N.S>A@$"));
            Assert.Throws<GameObjectNotFoundByPath>(() => SelectByPath<GameObject>("N.AJSJDJ2213"));
            __GO__.goStorage.Clear();
        }
    }

    public class UIText : Component
    {

    }
}
