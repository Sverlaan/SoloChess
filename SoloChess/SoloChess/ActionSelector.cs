using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloChess
{
    public class ActionSelector
    {
        LinkedList<ActionNode> nodes;
        double noemer;
        Random random;

        public ActionSelector(List<Move> moves)
        {
            random = new Random();
            nodes = new LinkedList<ActionNode>();

            noemer = 0;
            foreach(Move move in moves)
            {
                double teller = Math.Pow(Math.E, move.Value);
                noemer += teller;
                nodes.AddLast(new ActionNode(move, teller));
            }
        }

        public Move SampleAction()
        {
            double r = random.NextDouble();
            double sum = 0;

            ActionNode picked = null;
            foreach(ActionNode an in nodes)
            {
                if (sum + an.teller / noemer >= r)
                {
                    // an is picked
                    picked = an;
                    break;
                }
            }

            nodes.Remove(picked);
            noemer -= picked.teller;
            return picked.move;
        }

        public bool IsEmpty()
        {
            return nodes.Count == 0;
        }

    }

    public class ActionNode
    {
        public double teller;
        public Move move;

        public ActionNode(Move move, double teller)
        {
            this.move = move;
            this.teller = teller;
        }
    }
}
