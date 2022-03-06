using UnityEngine;
using System.Collections;

public class ScrollableCell : MonoBehaviour
{
    [SerializeField]
    private int dataIndex = -1;

    public int DataIndex {
        get { return dataIndex; }
    }

    //public void InitData(object data, int index)
    //{
    //    this.dataObject = data;
    //    this.dataIndex = index;

    //    if (data == null)
    //        this.gameObject.SetActive(false);
    //    else
    //        this.gameObject.SetActive(true);
    //}
    public void InitData(int index)
    {
        dataIndex = index;
        if (index == -1)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    public void ConfigureCell()
    {
        if (dataIndex != -1)
            this.ConfigureCellData();
    }

    public virtual void ConfigureCellData(){ 
    }

    private void OnDestroy()
    {
        CleanData();
    }
    public virtual void CleanData()
    {

    }
}
