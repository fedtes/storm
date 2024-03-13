using Storm.Schema;
using System;
using System.Collections.Generic;

namespace Storm.Origins
{
    /// <summary>
    /// A table/view in the from block. Hold information about the join condition and the tables/views that are joined with it.
    /// </summary>
    public class Origin
    {
        /// <summary>
        /// The full path of the origin
        /// </summary>
        public EntityPath FullPath;
        /// <summary>
        /// The current entity which supply the table/view information
        /// </summary>
        public Entity Entity;
        /// <summary>
        /// The link between this entity and the one in the parent origin. Has the information to generate the join condition. Can be null in case of root origin.
        /// </summary>
        public NavigationProperty Edge;
        /// <summary>
        /// Alias used in the query to refer to this table/view
        /// </summary>
        public String Alias;
        /// <summary>
        /// All the orgins that has an entity joined with this one. Can be null.
        /// </summary>
        public List<Origin> children;
    }
}
