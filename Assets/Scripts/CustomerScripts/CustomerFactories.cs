using UnityEngine;

public abstract class CustomerFactory
{
    public abstract GameObject CreateCustomer(Vector3 position, Quaternion rotation, CustomerPreset preset, int id);
    protected GameObject MakeObjectWithName(string fileNameOfPrefab, Vector3 position, Quaternion rotation, CustomerPreset preset, int idOfCustomer)
    {
        GameObject prefab = Resources.Load<GameObject>(fileNameOfPrefab);
        if (prefab == null)
        {
            Debug.Log($"{fileNameOfPrefab} not found!");
        }
        prefab.GetComponent<CustomerBehavior>().GiveOrder(preset, idOfCustomer);
        return Object.Instantiate(prefab, position, rotation);
    }

}

public class NormalCustomerFactory : CustomerFactory
{
    public override GameObject CreateCustomer(Vector3 position, Quaternion rotation, CustomerPreset preset, int id)
    {
        return MakeObjectWithName("Customer", position, rotation, preset, id);
    }
}

public class NamedCustomerFactory : CustomerFactory
{
    public override GameObject CreateCustomer(Vector3 position, Quaternion rotation, CustomerPreset preset, int id)
    {
        return MakeObjectWithName("NamedCustomer", position, rotation, preset, id);
    }
}
