using System;
using System.Windows.Controls;
using System.Collections;

namespace SLPropertyGrid
{
	public static class EditorService
	{
		public static ValueEditorBase GetEditor(PropertyItem propertyItem, PropertyGridLabel label)
		{
			if (propertyItem == null) throw new ArgumentNullException("propertyItem");

			var attribute = propertyItem.GetAttribute<EditorAttribute>();
			if (attribute != null)
			{
				var editorType = Type.GetType(attribute.EditorTypeName, true);
				if (editorType != null)
					return Activator.CreateInstance(editorType, label, propertyItem) as ValueEditorBase;
			}

			var propertyType = propertyItem.PropertyType;

			var editor = GetEditor(propertyType, label, propertyItem);

			while (editor == null && propertyType.BaseType != null)
			{
				propertyType = propertyType.BaseType;
				editor = GetEditor(propertyType, label, propertyItem);
			}

			return editor;
		}

		public static ValueEditorBase GetEditor(Type propertyType, PropertyGridLabel label, PropertyItem property)
		{
			if (typeof(Boolean).IsAssignableFrom(propertyType))
				return new BooleanValueEditor(label, property);

			if (typeof(Enum).IsAssignableFrom(propertyType))
				return new EnumValueEditor(label, property);

			if (typeof(DateTime).IsAssignableFrom(propertyType))
				return new DateTimeValueEditor(label, property);

			if (typeof(String).IsAssignableFrom(propertyType))
				return new StringValueEditor(label, property);

			if (typeof(ValueType).IsAssignableFrom(propertyType))
				return new StringValueEditor(label, property);

            if (typeof(IList).IsAssignableFrom(propertyType))
                return new CollectionValueEditor(label, property);

			if (typeof(Object).IsAssignableFrom(propertyType))
			    return new NestedObjectValueEditor(label, property);

			return new StringValueEditor(label, property);
		}

	}
}
