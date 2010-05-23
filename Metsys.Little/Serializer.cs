﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Metsys.Little
{
   public class Serializer
   {
      private static readonly IDictionary<Type, Action<BinaryWriter, object>>  _writerLookup = new Dictionary<Type, Action<BinaryWriter, object>>
         {
            {typeof(bool), (w, o) => w.Write((bool)o)},
            {typeof(int), (w, o) => w.Write((int)o)},
            {typeof(short), (w, o) => w.Write((short)o)},
            {typeof(long), (w, o) => w.Write((long)o)},
            {typeof(float), (w, o) => w.Write((float)o)},
            {typeof(double), (w, o) => w.Write((double)o)},
            {typeof(decimal), (w, o) => w.Write((decimal)o)},
            {typeof(byte), (w, o) => w.Write((byte)o)},
            {typeof(string), (w, o) => w.Write((string)o)},
            {typeof(char), (w, o) => w.Write((char)o)},
            {typeof(DateTime), (w, o) => w.Write(((DateTime)o).ToBinary())},
         };
      private readonly BinaryWriter _writer;

      public static byte[] Serialize<T>(T document)
      {
         using (var ms = new MemoryStream(250))
         using (var writer = new BinaryWriter(ms))
         {
            new Serializer(writer).WriteObject(document);
            return ms.ToArray();
         }
      }

      private Serializer(BinaryWriter writer)
      {
         _writer = writer;
      }

      private void WriteObject(object o)
      {
         var helper = TypeHelper.GetHelperForType(o.GetType());
         foreach(var property in helper.Properties)
         {
            var value = property.Getter(o);
            if (value == null)
            {
               if (!property.Nullable)
               {
                  throw new InvalidOperationException(string.Format("Got not value for non-nullable type: {0}", property.Type.Name));
               }
               WriteNull();
               continue;
            }
            SerializeMember(property.Getter(o), property.Nullable);
         }
      }
      private void SerializeMember(object value, bool nullable)
      {
         var type = value.GetType();
         if (type.IsEnum)
         {
            type = Enum.GetUnderlyingType(type);
         }
         if (nullable)
         {
            _writer.Write((byte)1);
         }
         Action<BinaryWriter, object> w;
         if (_writerLookup.TryGetValue(type, out w))
         {
            w(_writer, value);
         }
         else
         {
            WriteObject(value);  
         }
      }
      private void WriteNull()
      {
         _writer.Write((byte)0);
      }
   }
}