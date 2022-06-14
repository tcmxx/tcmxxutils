/* * * * *
 * Modified from
 * https://github.com/Bunny83/UUID/issues
 * * * * */

using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public sealed class UniqueIDComponent : MonoBehaviour, ISerializationCallbackReceiver {
    private static readonly Dictionary<string, UniqueIDComponent> IDToObj = new Dictionary<string, UniqueIDComponent>();

    private static void RegisterOrGenerateUniqueID(UniqueIDComponent idComponent) {

        if (string.IsNullOrEmpty(idComponent.id)) {
            // No ID yet, generate a new one.
            idComponent.id = System.Guid.NewGuid().ToString();
            IDToObj.Add(idComponent.id, idComponent);
            return;
        }

        if (!IDToObj.TryGetValue(idComponent.id, out var tempIdComponent)) {
            // ID not known to the DB, so just register it
            IDToObj.Add(idComponent.id, idComponent);
            return;
        }

        if (tempIdComponent == null) {
            IDToObj[idComponent.id] = idComponent;
            return;
        }

        if (tempIdComponent != idComponent) {
            // DB inconsistency, change current id
            idComponent.id = System.Guid.NewGuid().ToString();
            IDToObj.Add(idComponent.id, idComponent);
        }
    }

    [SerializeField]
    [ReadOnly]
    private string id = null;

    public string ID => id;

    public void OnBeforeSerialize() {
    }

    public void OnAfterDeserialize() {
        RegisterOrGenerateUniqueID(this);
    }
}