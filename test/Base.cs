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

        [Fact]
        public void Test02()
        {
            void SetupTest02()
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

            SetupTest02();


            Assert.Throws<GameObjectNotFoundByPath>(() =>  SelectByPath<UIText>("Canvas>Layout>InvalidObject>Title[UIText]"));


            __GO__.goStorage.Clear();
        }

        [Fact]
        public void Test03()
        {
            void SetupTest03()
            {
                /*
                %root% |
                    - Canvas |
                        - Layout |
                            - Header |
                                - Title | <UIText>
                            - Header |
                                - Title | <UIText>
                 */
                var canvas = new GameObject("Canvas");
                var layout = new GameObject("Layout");
                var header1 = new GameObject("Header") { metadata = "header1"};
                var header2 = new GameObject("Header") { metadata = "header2" };
                var title = new GameObject("Title");
                var uiText = new UIText();


                title.components.Add(uiText);
                header1.transform.Childs.Add(title);
                header2.transform.Childs.Add(title);
                layout.transform.Childs.Add(header1);
                layout.transform.Childs.Add(header2);
                canvas.transform.Childs.Add(layout);

                __GO__.Add(canvas);
            }

            SetupTest03();


            var gp0 = SelectByPath("Canvas>Layout>Header:(0)");
            var gp1 = SelectByPath("Canvas>Layout>Header:(1)");

            Assert.NotNull(gp0);
            Assert.NotNull(gp1);

            Assert.Equal("header1", gp0.metadata);
            Assert.Equal("header2", gp1.metadata);

            Assert.Throws<QueryIndexerException>(() => SelectByPath("Canvas>Layout>Header:(3)"));

            __GO__.goStorage.Clear();
        }
    }



    public class UIText : Component
    {

    }
}
