

using System.Collections.Generic;

namespace Intelligence
{
    public class AntBT : Tree
    {
        // Start is called before the first frame update
        void Start ()
        {
    
        }
    
        // Update is called once per frame
        void Update ()
        {
    
        }

        protected override Node SetUp ()
        {
            Node root = new NodeRoot(new List<Node>
            {
                new NodeSequence(new List<Node>
                {
                    new NodeNot(new List<Node>
                    {
                        //MyLeaveNode
                    }),
                    new NodeSequence(new List<Node>
                    {
                        new NodeNot(new List<Node>(
                            //MyLeaveNode2
                        ))
                        //MyLeaveNode3
                    })
                })
            });
            return root;
        }
    }
}