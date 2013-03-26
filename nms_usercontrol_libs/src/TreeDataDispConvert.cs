using nms_database_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_usercontrol_libs.src
{
    public class TreeDataDispConvert
    {
        /***********************************************************************************
         * 树形表初始化，从实际数据转成显示数据
         * ********************************************************************************/
        public void TreeViewInitial(List<Tree> treeLists, List<PropertyNodeItem> nodeItemLists)
        {
            foreach (Tree element in treeLists)
            {
                if (null == element)
                    continue;

                //--------- 判断此ID 是否存在 --------------------------------
                if (true == PropertyNodeExsits(nodeItemLists, element.ID))
                {
                    continue;
                }

                PropertyNodeItem nodeItem = new PropertyNodeItem()
                {
                    Id = element.ID,                    
                    DisplayName = element.DispName,
                    NodeId = element.NodeID,
                    NodeType = element.NodeType,
                };

                //------------- 填充显示图标 --------------------------------
                if (nodeItem.NodeType == (int)TreeNodeType.SiteType)
                {
                    nodeItem.Icon = PropertyNodeItem.RPTICON3;
                }
                else if (nodeItem.NodeType == (int)TreeNodeType.AreaType)
                {
                    nodeItem.Icon = PropertyNodeItem.CITYICON;
                }
                else
                {
                    nodeItem.Icon = PropertyNodeItem.CITYICON;
                }

                //--------------- 获取子节点 ----------------------------------
                List<Tree> childrens = GetChildrenNodeItems(treeLists, element.ID);
                foreach (Tree item in childrens)
                {
                    PropertyNodeItem children = new PropertyNodeItem()
                    {
                        Id = item.ID,
                        NodeId = item.NodeID,
                        NodeType = item.NodeType,
                    };
                    nodeItem.Children.Add(children);
                }

                nodeItemLists.Add(nodeItem);
            }
        }


        /**************************************************************************
         * 找出子节点
         * ***********************************************************************/
        private List<Tree> GetChildrenNodeItems(List<Tree> treeList, int id)
        {
            List<Tree> list = new List<Tree>();

            foreach (Tree item in treeList)
            {
                if (item.NodePID == id)
                {
                    list.Add(item);
                }
            }

            return list;
        }
        /**************************************************************************
         * 查找当前ID是否已经被取出
         * ***********************************************************************/
        private bool PropertyNodeExsits(List<PropertyNodeItem> treeListView, int id)
        {
            bool retval = false;

            foreach (PropertyNodeItem element in treeListView)
            {
                if (null == element)
                    continue;

                if (element.Id == id)
                    return true;

                if (element.Children.Count <= 0)
                    return false;

                retval = PropertyNodeExsits(element.Children, id);
            }

            return retval;
        }
    }
}
