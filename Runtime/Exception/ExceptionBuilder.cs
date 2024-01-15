using System;
using System.IO;

namespace EZ.DataTool
{
    [Serializable]
    public class DataException : SystemException 
    {
        public DataException() 
        {
        }

        public DataException(string s) : base(s)
        { 
        }
    }

    [Serializable]
    public class DuplicateNameException : DataException 
    {
        public DuplicateNameException()
        {
        }

        public DuplicateNameException(string s) : base(s)
        {
        }
    }

    [Serializable]
    public class InvalidConstraintException : DataException
    {
        public InvalidConstraintException()
        {
        }

        public InvalidConstraintException(string s) : base(s)
        {
        }
    }

    [Serializable]
    public class MissingPrimaryKeyException : DataException
    {
    }

    [Serializable]
    public class RowNotInTableException : DataException 
    {
    }

    [Serializable]
    public class ColumnNotInAnyTable : DataException
    {
    }

    [Serializable]
    public class ColumnNotInTableException : DataException
    {
        public ColumnNotInTableException()
        {
        }

        public ColumnNotInTableException(string s) : base(s)
        {
        }
    }

    public static class ExceptionBuilder
    {
        static private IndexOutOfRangeException _IndexOutOfRange(string error)
        {
            return new IndexOutOfRangeException(error);
        }

        static private DuplicateNameException _DuplicateName(string error)
        {
            return new DuplicateNameException(error);
        }

        static private InvalidConstraintException _InvalidConstraint(string error)
        {
            return new InvalidConstraintException(error);
        }

        static public FileNotFoundException FileNotFound(string message, string fileName)
        {
            return new FileNotFoundException(message, fileName);
        }

        static public DuplicateNameException DuplicateTableName(string table)
        {
            return _DuplicateName($"DataTable DuplicatedName: {table}");
        }

        static public DuplicateNameException DuplicateColumnName(string column)
        {
            return _DuplicateName($"DataColumn DuplicatedName: {column}");
        }

        static public MissingPrimaryKeyException MissingPrimaryKey()
        {
            return new MissingPrimaryKeyException();
        }

        static public RowNotInTableException RowNotInTable()
        {
            return new RowNotInTableException();
        }

        static public Exception KeyTooManyColumns()
        {
            return _InvalidConstraint($"KeyTooManyColumns");
        }

        static public ColumnNotInTableException ColumnNotInTable(string column)
        {
            return new ColumnNotInTableException(column);
        }

        static public ColumnNotInAnyTable ColumnNotInAnyTable()
        {
            return new ColumnNotInAnyTable();
        }

        static public IndexOutOfRangeException ColumnOutOfRange(int index)
        {
            return _IndexOutOfRange($"ColumnOutOfRange: {index}");
        }
    }
}