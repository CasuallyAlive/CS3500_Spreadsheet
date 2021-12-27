// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

// Co-Author: Jordy A. Larrea Rodriguez (U1236145)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;
        private int count;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            this.dependents = new Dictionary<string, HashSet<string>>();
            this.dependees = new Dictionary<string, HashSet<string>>();
            this.count = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return this.count; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (this.dependees.ContainsKey(s))
                    return this.dependees[s].Count;
                return 0;
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (!this.dependents.ContainsKey(s))
                return false;

            return dependents[s].Count > 0;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (!this.dependees.ContainsKey(s))
                return false;
            return this.dependees[s].Count > 0;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!HasDependents(s))
                return new HashSet<string>();
            return new HashSet<string>(this.dependents[s]);
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (!HasDependees(s))
                return new HashSet<string>();
            return new HashSet<string>(this.dependees[s]);
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        /// t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (nodeExists(s, t))
                return;
            if (this.dependents.ContainsKey(s))
                dependents[s].Add(t); // If node s is already in the dependency list, then add t to the list of dependents of s; otherwise, create a node for s.
            else
                createNode(s, t, null);
            if (this.dependents.ContainsKey(t))
                this.dependees[t].Add(s); // If node t is already in the dependency list, then add s to the list of dependees of t; otherwise, create a node for t.
            else
                createNode(t, null, s);
            this.count++;
        }

        /// <summary>
        /// Creates a new node with lists of either dependents or dependees
        /// </summary>
        /// <param name="node"></param>
        /// <param name="dependent"></param>
        /// <param name="dependee"></param>
        private void createNode(string node, string dependent, string dependee)
        {
            this.dependents.Add(node, newList(dependent));
            this.dependees.Add(node, newList(dependee));
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists. Furthermore, the method removes a given node if the node becomes irrelevant (has no dependencies and dependents).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if(!nodeExists(s, t))
                return;
            this.dependents[s].Remove(t);
            this.dependees[t].Remove(s);

            if(this.dependents[s].Count == 0 && this.dependees[s].Count == 0)
                removeNode(s);
            if(this.dependents.ContainsKey(t) && this.dependents[t].Count == 0 && this.dependees[t].Count == 0)
                removeNode(t);
            this.count--;
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            foreach (string dependent in GetDependents(s))// for each dependent in the list of dependents belonging to s, remove the link between the dependent and s
                this.RemoveDependency(s, dependent);
            foreach (string dependent in newDependents)// add the new dependents to s and create nodes for said dependents
                this.AddDependency(s, dependent);
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            foreach (string dependee in GetDependees(s)) // for each dependee in the list of dependees belonging to s, remove the link between the dependee and s
                RemoveDependency(dependee, s);
            foreach (string dependee in newDependees) // add the new dependees to s and create nodes for said dependees
                this.AddDependency(dependee, s);
        }

        /// <summary>
        /// Checks if a node exists in this dependency graph.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool nodeExists(string s, string t)
        {
            return this.dependents.ContainsKey(s) && this.dependents[s].Contains(t);
        }
        /// <summary>
        /// Removes a node completely from the dependency graph
        /// </summary>
        /// <param name="node"></param>
        private void removeNode(string node)
        {
            this.dependents.Remove(node);
            this.dependees.Remove(node);
        }

        /// <summary>
        /// Creates a new Hash-Set with an initial value from the param 'firstString'.  
        /// </summary>
        /// <param name="firstString"></param>
        /// <returns></returns>
        private HashSet<string> newList(string firstString)
        {
            HashSet<string> values = new HashSet<string>();
            if (firstString != null)
                values.Add(firstString);
            return values;
        }
    }

}

